# Informe de pruebas

## 1. Introducción

Este informe documenta los resultados de pruebas realizadas sobre tres modelos de autoridad en red:

1. **Server Authority** — El servidor es el único que valida y ejecuta el movimiento
2. **Server Authority + Rewind** — El cliente predice localmente y el servidor valida
3. **Client Authority** — El cliente tiene autoridad directa sobre su movimiento

Se evalúan combinadas con presencia o ausencia de **interpolación** en el `NetworkTransform`, bajo diferentes condiciones de red simuladas.

He utilizado **Multiplayer Play Mode (MPPM)** para instancias simultáneas del editor y **Multiplayer Tools** para simular las diferentes condiciones de la red (latencia, pérdida de paquetes y jitter).

## 2. Resultados de las pruebas v3.0
Para percibir la diferencia entre autoridad en el servidor y en el cliente hay que cambiar las condiciones de red al menos a Mobile 5G y algo de lag (al menos 1000ms).
Con el cambio se ve que en autoridad en el servidor se mueve primero el player del lado del servidor y después del lado del cliente y viceversa.

Cuando añadimos interpolación los movimientos son más suaves y con rewind el player debe "ver" si puede situarse donde quiere, es decir, si el espacio que quiere ocupar está libre puede moverse y si ya está ocupado debe reposicionarse.

## 3. Resultados de las pruebas v5.0
En la versión 5.0 he utilizado la IA para encontrar una solución distintal al cambio de la propiedad **Authority Mode** en caliente del Network Transform (v3.0), añadiendo un script **Dynamic Network Transform** que hereda del anterior y haciendo en Player Controller un `OnNetworkSpawn` que fuerza a reevaluar quien tiene la autoridad.
Aún con todo esto, el resultado en las pruebas es el mismo, necesitando poner una red Mobile 5G y al menos 1000ms de lag para poder ver con claridad quien tiene la autoridad y la prioridad de movimiento.