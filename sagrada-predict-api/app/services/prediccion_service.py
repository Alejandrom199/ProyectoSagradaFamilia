import pandas as pd
from prophet import Prophet
from datetime import datetime
from dateutil.relativedelta import relativedelta

from app.schemas.prediccion import (
    PrediccionRequest,
    PrediccionResponse,
    PuntoPrediccion
)

class PrediccionService:

    MINIMO_MEDIDAS = 3

    def predecir_peso(self, solicitud: PrediccionRequest) -> PrediccionResponse:

        if len(solicitud.medidas) < self.MINIMO_MEDIDAS:
            return PrediccionResponse(
                puede_predecir = False,
                nino_id        = solicitud.nino_id,
                mensaje        = f"Se necesitan al menos {self.MINIMO_MEDIDAS} "
                                 f"medidas. Hay {len(solicitud.medidas)}."
            )

        df = pd.DataFrame({
            "ds":    pd.to_datetime([m.fecha for m in solicitud.medidas]),
            "y":     [m.peso for m in solicitud.medidas],
            "cap":   solicitud.cap,
            "floor": solicitud.floor
        })

        # Creación y entrenamiento del modelo
        modelo = Prophet(
            growth             = "logistic",
            interval_width     = 0.80,
            yearly_seasonality = False,
            weekly_seasonality = False,
            daily_seasonality  = False
        )
        modelo.fit(df)

        # Fechas futuras: 3, 6 y 12 meses desde fecha actual 
        hoy = datetime.today()
        futuro = pd.DataFrame({
            "ds": [
                hoy + relativedelta(months=3),
                hoy + relativedelta(months=6),
                hoy + relativedelta(months=12),
            ],
            "cap":   solicitud.cap,
            "floor": solicitud.floor
        })

        forecast = modelo.predict(futuro)

        # Respuesta
        meses_list   = [3, 6, 12]
        predicciones = []

        for i, meses in enumerate(meses_list):
            fila = forecast.iloc[i]
            predicciones.append(PuntoPrediccion(
                meses    = meses,
                fecha    = fila["ds"].strftime("%Y-%m-%d"),
                predicho = round(float(fila["yhat"]),       2),
                minimo   = round(float(fila["yhat_lower"]), 2),
                maximo   = round(float(fila["yhat_upper"]), 2)
            ))

        return PrediccionResponse(
            puede_predecir = True,
            nino_id        = solicitud.nino_id,
            predicciones   = predicciones
        )