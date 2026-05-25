import os

class Settings:
    APP_NAME    = "Sagrada Predict API"
    VERSION     = "1.0.0"
    DESCRIPTION = "Microservicio de predicción de crecimiento infantil - Consultorio La Sagrada Familia"

    # os.getenv(key, default) lee primero la variable de entorno inyectada por
    # Docker/docker-compose; si no existe, cae al valor por defecto local.
    # Esto permite sobreescribir cualquier parámetro sin reconstruir la imagen.

    PORT            = int(os.getenv("PORT",            "8000"))

    # En Docker, 'api' es el nombre del servicio .NET en la red interna.
    # Localmente, mantener http://localhost:5001 como fallback de desarrollo.
    NET_BACKEND_URL = os.getenv("NET_BACKEND_URL", "http://localhost:5001")

    MINIMO_MEDIDAS  = int(os.getenv("PROPHET_MIN_MEDIDAS",      "3"))
    INTERVAL_WIDTH  = float(os.getenv("PROPHET_INTERVAL_WIDTH", "0.80"))
    CAP_DEFAULT     = float(os.getenv("PROPHET_CAP_DEFAULT",    "22.0"))
    FLOOR_DEFAULT   = float(os.getenv("PROPHET_FLOOR_DEFAULT",  "2.5"))

settings = Settings()