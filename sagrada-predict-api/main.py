from fastapi import FastAPI
from app.routers import prediccion
from app.core.config import settings
from fastapi.encoders import jsonable_encoder
from fastapi.responses import JSONResponse

app = FastAPI(
    title       = settings.APP_NAME,
    version     = settings.VERSION,
    description = settings.DESCRIPTION
)

# Registrar los routers
app.include_router(prediccion.router)

@app.get("/health", tags=["Sistema"])
def health():
    return {
        "status":  "ok",
        "service": settings.APP_NAME,
        "version": settings.VERSION
    }