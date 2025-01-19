# Anteproyecto

## 1.1. Idea

- El proyecto consiste en el desarrollo completo de un videojuego de plataformas 2D, desde la planificación y diseño hasta la creación del entregable final.

- El proyecto consiste en crear un videojuego de plataformas 2D donde el jugador controla a un dinosaurio en una aventura emocionante. El propósito principal es proporcionar una experiencia de juego divertida y desafiante, donde el jugador recoja monedas y evite enemigos para avanzar en los niveles. Además, el jugador intentará obtener la máxima puntuación posible para entrar en el ranking de los mejores jugadores.

- La aplicación está destinada a jugadores de todas las edades que disfrutan de los juegos de plataformas y buscan una experiencia de juego nostálgica similar a los juegos arcade clásicos.

- El proyecto pretende satisfacer la necesidad de entretenimiento y desafío para los jugadores. Aunque no es nuestra intención comercializar el juego, su desarrollo abre una oportunidad de negocio al poder comercializarlo a través de plataformas de distribución digital como Play Store o mediante la venta directa del juego.

- Existen muchos juegos de plataformas 2D que buscan satisfacer la necesidad de entretenimiento y desafío. Algunos ejemplos son “Super Mario Bros.”, “Celeste” y “Donkey Kong”. Estos juegos tienen éxito en proporcionar experiencias de juego envolventes y desafiantes.

- Los objetivos del proyecto son crear un juego de plataformas 2D divertido y desafiante, proporcionar una experiencia de juego fluida e inmersa y ofrecer gráficos vibrantes y coloridos, incluyendo música de fondo y efectos de sonido que complementen la acción en pantalla.
Los requisitos básicos que debe cumplir son la compatible con los dispositivos, el juego debe permitir el control total del dinosaurio por el jugador y debe incluir diferentes niveles con desafíos crecientes.

- Para el desarrollo del juego, se empleará un motor de juego como Unity. Además, se utilizará un lenguaje de programación como C#, un editor de código como el Visual Studio Code y un sistema de persistencia.

## 1.2. Descripción y tipo

El proyecto a desarrollar es un videojuego de plataformas 2D para ordenador Windows. En este juego de plataformas 2D, el jugador se embarcará en una aventura emocionante controlando a un dinosaurio. Desde el momento en que el jugador entra en el mundo del juego, se encontrará inmerso en un entorno lleno de desafíos y recompensas.

Visualmente, el jugador se encontrará en un mundo vibrante y colorido, lleno de plataformas para saltar, escaleras para subir y bajar terrenos, rampas, monedas para recoger y enemigos para evitar. El dinosaurio, controlado por el jugador, se moverá fluidamente por el escenario para recoger todas las monedas, ya que no se podrá pasar de nivel sin recogerlas. Habrá dos tipos de monedas: las amarillas y las rojas. Para pasar de nivel, solo se necesitará recoger las monedas amarillas, mientras que las rojas otorgarán una puntuación adicional.
Las monedas amarillas darán 10 puntos cada una, y las rojas, que serán más difíciles de conseguir, otorgarán 30 puntos. Al final del juego, la puntuación total servirá para comparar quién tiene la mejor puntuación, similar a los juegos de arcade clásicos donde al final aparecía una pantalla en negro con el nombre y la puntuación de los jugadores.

Auditivamente, el jugador estará acompañado por una música de fondo que complementa la acción en pantalla. Los efectos de sonido, como el sonido de las monedas al ser recogidas o el sonido de los enemigos al ser derrotados, añadirán una capa adicional de inmersión a la experiencia de juego. En cuanto a la interactividad, el jugador tendrá un control total sobre el dinosaurio. Podrá moverse a la  izquierda y a la derecha, subir y bajar escaleras, saltar para superar obstáculos y recoger monedas. Además, el jugador deberá evitar a los enemigos, las trampas y no caerse al vacío para no perder vidas.

A lo largo del juego, el jugador se enfrentará a niveles cada vez más difíciles, pero también tendrá la satisfacción de ver cómo su contador de monedas aumenta y cómo supera cada nivel. Y si en algún momento el juego se vuelve demasiado desafiante, el jugador siempre puede pausar el juego y retomarlo más tarde.

## 1.3. Requisitos funcionales

- **Control del Personaje**:
El jugador debe poder mover al dinosaurio hacia la izquierda, derecha, saltar y agacharse. El dinosaurio debe poder recoger monedas al tocarlas. El dinosaurio debe poder derrotar enemigos al saltar sobre ellos.

- **Niveles del Juego**:
El juego debe tener 4 niveles distintos, cada uno con un diseño único y desafíos específicos. Cada nivel debe tener un objetivo claro, como recolectar todas las monedas para avanzar al siguiente nivel.

- **Interfaz de Usuario**:
Debe haber una pantalla de inicio con opciones para iniciar el juego, ver las instrucciones y acceder a la configuración. Durante el juego, debe mostrarse el número de monedas recolectadas y las vidas del personaje.

- **Música y Efectos de Sonido**:
Debe haber música de fondo que se reproduzca durante el juego.Deben incluirse efectos de sonido para acciones como recoger monedas y derrotar enemigos.

- **Guardar Puntuaciones**:
El juego debe permitir a los jugadores guardar su puntuación al finalizar el juego o cuando el personaje muera en cualquier nivel. La puntuación guardada debe ser comparada con las puntuaciones anteriores y actualizarse en una tabla de clasificación. La tabla de clasificación debe mostrar siempre al jugador con la puntuación más alta en la primera posición.

## 1.4. Requisitos no funcionales

- **Rendimiento**: El juego debe cargar en menos de 5 segundos.

- **Usabilidad**: La interfaz debe ser intuitiva y fácil de navegar. Los controles deben ser responsivos y fáciles de usar.

- **Compatibilidad**: El juego debe ser compatible con ordenadores con sistema operativo Windows.

- **Mantenibilidad**: El código del juego debe estar bien documentado para facilitar futuras actualizaciones y mantenimiento.

### 1.5. Tecnología

Por lo tanto, las tecnologías y herramientas a emplear serán:

| **Desarrollo del juego** | Unity |
|----------|----------|
| **Editor de código**| Visual Studio Code  |
| **Lenguaje de programación**|C#|
| **Persistencia de datos** | Sistema de persistencia de Unity(PlayerPrefs o un sistema de archivos)   |

## 1.6. Estructura

El juego constará de distintas pantallas, una pantalla de Menú ofrece tres opciones: “Salir”, “Comenzar”,“Clasificación” y “Opciones”. Si se elige “Opciones” se irá a una pantalla de Opciones, el jugador podrá personalizar su experiencia de juego ajustando el volumen de la música y los efectos de sonido. Si se elige “Comenzar” se irá a una pantalla de instrucciones donde se proporciona al jugador las instrucciones del juego y los controles necesarios, presentados en formato de texto e imágenes. Luego habrá 4 pantallas por cada nivel, cada uno con sus propios diseños y desafíos, y por último la pantalla final o ”Clasificación”, donde aparecerá una lista con la clasificación de los jugadores y sus puntuaciones. Esta pantalla se puede acceder desde la pantalla Menú y también aparecerá cada vez que se finalice el juego o muera.

## 1.7. Proyección futura

- ***Implementación de opciones multilingües***
Se prevé hacer una incorporación de una opción de cambio de idioma. Se planea ofrecer al menos inglés y castellano, con la posibilidad de añadir gallego en el futuro. Esta funcionalidad permitirá a los usuarios seleccionar su idioma preferido, mejorando la accesibilidad y la experiencia de juego para una audiencia más amplia.

- ***Adaptación del juego para tabletas***
Se considerará hacer una adaptación para tabletas. Dado que las pantallas de las tabletas son más grandes que las de los móviles, pero más pequeñas que las de los ordenadores, será necesario ajustar la relación de aspecto y la disposición de los elementos en pantalla para asegurar que los ítems no cambien de lugar y se mantenga la jugabilidad. Además, se implementarán “teclas virtuales” en la pantalla táctil para permitir el movimiento y los saltos del personaje, para aquellas tabletas que no dispongan de teclado.

- ***Funcionalidad online y sistema de inicio de sesión***
Otra mejora importante será la implementación de una funcionalidad online que permita a los jugadores iniciar sesión en el juego. Esta funcionalidad será esencial para la clasificación y el ranking de los mejores jugadores. Al ser online, permitirá que todos los jugadores, independientemente del dispositivo desde el que jueguen, puedan competir y ver su posición en una clasificación global.
