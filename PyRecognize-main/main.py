import asyncio

from Models.recorder import Recorder, RecorderConfig
from Models.speech_recognize import VoskModel
from config import config

async def main():
    model = VoskModel(config.vosk_small_model_path)
    recorder = Recorder(
        record_config=RecorderConfig(config.sky_model_path),
        vosk_model=model
    )

    await recorder.run()


if __name__ == "__main__":
    asyncio.run(main())
