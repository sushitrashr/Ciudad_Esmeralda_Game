# Ciudad Esmeralda – Prototipo de Videojuego 2D

**Enlace al Documento del Proyecto:** [Ver Informe en Google Drive](https://drive.google.com/file/d/1fspa4Rx1iXBOr-fAMSki3ixtvRoovGX2/view?usp=sharing)

## Sinopsis

**Ciudad Esmeralda** es un videojuego en 2D ambientado en una ciudad distópica dominada por la opresión, la pobreza y la contaminación. La protagonista, **Harumi**, es una joven rebelde que ha perdido a su familia a causa del sistema corrupto y que emprende un viaje para liberar a su pueblo de las injusticias.

Durante su travesía, Harumi deberá atravesar zonas contaminadas, minas controladas por caballeros corruptos y barrios marginados, enfrentando pruebas físicas y morales que determinarán el destino de toda la ciudad.

## Mecánicas de Juego

* **Exploración y Decisiones:** Interacción con NPCs que ofrecen misiones y objetos clave.
* **Combate:** Enfrentamientos contra enemigos menores y caballeros de élite utilizando ataques cuerpo a cuerpo y armas improvisadas.
* **Aliados como soporte:** Entregan objetos, consejos o enseñanzas nuevas.
* **Impacto moral:** Las decisiones del jugador cambian el desenlace del juego.

## Construcción del Mundo

### Personaje Principal: Harumi
Joven de espíritu indomable que busca inspirar resistencia en su pueblo.
* **Habilidades:** Combate versátil, resistencia a entornos tóxicos con equipo especial y capacidad de inspirar a los habitantes.

### Aliados Secundarios (NPCs)
* **Maya:** Mentora en liderazgo y organización.
* **Nicanor:** Proveedor de tecnología verde (filtros, mejoras).
* **Minerva:** Maestra de nuevas habilidades.
* **León:** Informante sobre el poder del rey.

### Enemigos: Los Caballeros del Rey
Símbolos de diferentes formas de opresión.
* **Caballero del Humo:** Custodia zonas contaminadas.
* **Caballero de las Cadenas:** Controla las minas y esclavos.
* **Caballero de la Falsedad:** Manipula con propaganda.
* **Jefe Final – El Caballero del Absoluto:** Mano derecha del rey. Su derrota abre la posibilidad de liberar a la ciudad.

### Zonas del Juego
1.  Distrito Tóxico.
2.  Las Minas del Silencio.
3.  El Barrio de los Olvidados.
4.  Fortaleza Real.

---

## Detalles Técnicos del Prototipo en Unity

Este prototipo demuestra cómo los recursos técnicos transmiten la opresión del sistema y la lucha de Harumi.

### Elementos Implementados

1.  **Sistema de Animaciones (Harumi):**
    * Movimiento: Idle, Run, Walk, Jump.
    * Combate: Attack1, Attack2, Attack4, Shield (defensa).
    * Estado: Hurt (daño recibido), Dead (muerte).
2.  **Escenario Interactivo (Minas):** Prototipo funcional del nivel.
3.  **Rotación de objetos:** Engranajes y ventiladores giran en bucle usando `transform.Rotate()`, reforzando la atmósfera industrial.
4.  **Escala interactiva:** Cofres que aumentan de tamaño al abrirse y vuelven a su forma normal al cerrarse para dar retroalimentación visual.
5.  **Loops y Partículas:** Humo constante (Particle System), patrullaje de guardias y balanceo de cadenas para mantener el escenario vivo y tenso.

---

## Posibles Finales

* **Final Positivo (Libertad de SombraViva):** El pueblo se levanta tras la victoria de Harumi.
* **Final Neutro (Rebelión Incompleta):** Harumi vence, pero la ciudad sigue dividida.
* **Final Negativo (El ciclo de opresión):** Harumi fracasa y el dominio del rey se fortalece.
