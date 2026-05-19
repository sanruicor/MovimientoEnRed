# Informe de pruebas

## 1. Introducción

Este informe documenta los resultados de pruebas realizadas sobre tres modelos de autoridad en red:

1. **Server Authority** — El servidor es el único que valida y ejecuta el movimiento
2. **Server Authority + Rewind** — El cliente predice localmente y el servidor valida
3. **Client Authority** — El cliente tiene autoridad directa sobre su movimiento

Se evalúan combinadas con presencia o ausencia de **interpolación** en el `NetworkTransform`, bajo diferentes condiciones de red simuladas.

He utilizado **Multiplayer Play Mode (MPPM)** para instancias simultáneas del editor y **Multiplayer Tools** para simular las diferentes condiciones de la red (latencia, pérdida de paquetes y jitter).

## 2. Resultados de las pruebas
Para percibir la diferencia entre autoridad en el servidor y en el cliente hay que cambiar las condiciones de red al menos a Mobile 5G y algo de lag (al menos 1000ms).
Con el cambio se ve que en autoridad en el servidor se mueve primero el player del lado del servidor y después del lado del cliente y viceversa.
