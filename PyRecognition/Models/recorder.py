import logging
import pvporcupine
import websockets
from pvrecorder import PvRecorder
from Models.speech_recognize import VoskModel
from config import config

websocket_url = "ws://localhost:5001"

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

    async def start_recording(self) -> None:
        self.recorder.start()
        logger.info("Start listening...")
        try:
            while True:
                audio_frame = self.recorder.read()
                keyword_index = self.porcupine.process(audio_frame)
                if keyword_index >= 0:
                    await self.process()
        except Exception as e:
            logger.error(f"An error occurred during recording: {e}")
        finally:
            self.stop_and_cleanup()

    def stop_and_cleanup(self) -> None:
        self.recorder.stop()
        self.porcupine.delete()
        self.recorder.delete()

    async def run(self) -> None:
        await self.start_recording()

    async def send_message(self, message: str) -> None:
        try:
            async with websockets.connect(websocket_url) as websocket:
                await websocket.send(message)
        except Exception as e:
            logger.error(f"Error while sending data over WebSocket: {e}")

    async def process(self) -> None:
        await self.send_message("success")
        logger.info("I'm listening...")
        result = await self.vosk_model.run()
        logger.info(f"Processed: {result}")
        try:
            async with websockets.connect(websocket_url) as websocket:
                await websocket.send(result)
                logger.info(f"Sent result: {result}")
        except Exception as e:
            logger.error(f"Error while sending data over WebSocket: {e}")
