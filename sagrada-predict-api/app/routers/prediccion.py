from fastapi import APIRouter
from app.schemas.prediccion import PrediccionRequest, PrediccionResponse
from app.services.prediccion_service import PrediccionService

router = APIRouter(
    prefix="/api/prediccion",
    tags=["Predicción"]
)

servicio = PrediccionService()

@router.post("/peso", response_model=PrediccionResponse)
def predecir_peso(solicitud: PrediccionRequest):
    """
    Recibe el historial de medidas de un niño
    y devuelve predicciones de peso a 3, 6 y 12 meses.
    """
    return servicio.predecir_peso(solicitud)