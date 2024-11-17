import logging
import pvporcupine
from pvrecorder import PvRecorder
from Models.speech_recognize import VoskModel
from config import config

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

class RecorderConfig:
    def __init__(self, keyword_paths: str = None) -> None:
        self.keywords = [word for word in pvporcupine.KEYWORDS]
        self.porcupine = self.create_porcupine(keyword_paths)
        self.recorder = self.create_recorder()

    def create_porcupine(self, keyword_paths: str):
        return pvporcupine.create(
            access_key=config.picovoice_access_key,
            keyword_paths=[keyword_paths] if keyword_paths else self.keywords
        )

    def create_recorder(self) -> PvRecorder:
        return PvRecorder(
            device_index=-1,
            frame_length=self.porcupine.frame_length
        )


class Recorder:
    def __init__(self, record_config: RecorderConfig, vosk_model: VoskModel) -> None:
        self.record_config = record_config
        self.recorder = self.record_config.recorder
        self.porcupine = self.record_config.porcupine
        self.vosk_model = vosk_model
        self.websocket = None
        self.is_recording = False

    async def start_recording(self) -> None:
        self.is_recording = True
        self.recorder.start()
        logger.info("Start listening...")
        try:
            while self.is_recording:
                audio_frame = self.recorder.read()
                keyword_index = self.porcupine.process(audio_frame)
                if keyword_index >= 0:
                    await self.process()
        except Exception as e:
            logger.error(f"An error occurred during recording: {e}")
        finally:
            await self.stop_and_cleanup()

    async def stop(self) -> None:
        logger.info("Stopping recording...")
        self.is_recording = False
        await self.stop_and_cleanup()

    async def stop_and_cleanup(self) -> None:
        if self.recorder.is_recording:
            self.recorder.stop()
        self.porcupine.delete()
        self.recorder.delete()
        logger.info("Recorder and Porcupine cleaned up.")

    async def run(self, ws) -> None:
        self.websocket = ws
        await self.start_recording()

    async def process(self) -> None:
        logger.info("I'm listening...")
        result = await self.vosk_model.run()
        await self.websocket.send(result)
