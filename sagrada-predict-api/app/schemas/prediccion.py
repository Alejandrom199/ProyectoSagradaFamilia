from pydantic import BaseModel

# Request
class MedidaRequest(BaseModel):
    fecha: str
    peso:  float

class PrediccionRequest(BaseModel):
    nino_id: int
    sexo:    str
    medidas: list[MedidaRequest]
    cap:     float
    floor:   float

# Response
class PuntoPrediccion(BaseModel):
    meses:    int
    fecha:    str
    predicho: float
    minimo:   float
    maximo:   float

class PrediccionResponse(BaseModel):
    puede_predecir: bool
    nino_id: int
    mensaje: str | None = None
    predicciones: list[PuntoPrediccion] = []

    class Config:
        populate_by_name = True