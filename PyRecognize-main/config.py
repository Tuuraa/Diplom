class Config:
    _instance = None
    def __new__(cls):
        if cls._instance is None:
            cls._instance = super(Config, cls).__new__(cls)
            cls._instance.picovoice_access_key = "pxmn+C3eEEJ7PjTMvY9EtdRpsx61WKdQPfMgPVJOoR4m2128dyiEeg=="
            cls._instance.sky_model_path = "./ReModels/hey_sky_model.ppn"
            cls._instance.vosk_small_model_path = "./ReModels/vosk_small_model"
            cls._instance.websoket_url = "ws://192.168.0.15:5001"
        return cls._instance


config = Config()