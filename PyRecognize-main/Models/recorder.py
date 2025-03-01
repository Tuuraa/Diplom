import logging
import pvporcupine
import websockets
from pvrecorder import PvRecorder
from Models.speech_recognize import VoskModel
from config import config


logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)


class RecorderConfig:
    def __init__(self, keyword_paths: str = None) -> None:
        self.keywords = [word for word in pvporcupine.KEYWORDS]
        self.porcupine = self._create_porcupine(keyword_paths)
        self.recorder = self._create_recorder()

    def _create_porcupine(self, keyword_paths: str) -> pvporcupine.Porcupine:
        return pvporcupine.create(
            access_key=config.picovoice_access_key,
            keyword_paths=[keyword_paths] if keyword_paths else self.keywords
        )

    def _create_recorder(self) -> PvRecorder:
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
                    await self._process_wake_word()
        except Exception as e:
            logger.error(f"An error occurred during recording: {e}")
        finally:
            self._stop_and_cleanup()

    def _stop_and_cleanup(self) -> None:
        self.recorder.stop()
        self.porcupine.delete()
        self.recorder.delete()
        logger.info("Recording stopped and resources cleaned up.")

    async def run(self) -> None:
        await self.start_recording()

    async def _process_wake_word(self) -> None:
        logger.info("Wake word detected. Listening...")
        await self._send_websocket_message("success_wake_word")

        result = await self.vosk_model.run()
        await self._send_websocket_message(result)

    async def _send_websocket_message(self, message: str) -> None:
        try:
            async with websockets.connect(config.websoket_url) as websocket:
                await websocket.send(message)
                logger.info(f"Sent message: {message}")
        except Exception as e:
            logger.error(f"Error while sending data over WebSocket: {e}")