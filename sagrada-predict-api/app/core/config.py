class Settings:
    APP_NAME        = "Sagrada Predict API"
    VERSION         = "1.0.0"
    DESCRIPTION     = "Microservicio de predicción de crecimiento infantil - Consultorio La Sagrada Familia"
    
    # Puerto
    PORT            = 8000
    
    # URL de tu backend
    NET_BACKEND_URL = "http://localhost:5001"
    
    # Configuración de Prophet
    MINIMO_MEDIDAS      = 3
    INTERVAL_WIDTH      = 0.80
    
    # Límites biológicos por defecto si OMS no está disponible
    CAP_DEFAULT         = 22.0
    FLOOR_DEFAULT       = 2.5

# Instancia
settings = Settings()