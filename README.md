# Buenas Prácticas tenidas en cuenta
- Los assets los agrupé en un atlas para bajar los batches

- Todo lo que no sea movido durante runtime lo dejé como estático dentro de la escena, así es precargado

- Cuando puedo hago arrays y no listas porque los arrays ocupan la memoria de forma más ordenada y no necesito modificar el tamaño durante runtime. Las listas que uso se deben a 2 razones:
	- No conozco la cantidad inicial de objetos que habrá en la lista
	- Debo modificar el tamaño en runtime

- Campos con nombres comprensibles

# SOLID
- S:
  
	***Separación en scripts y explicación de cada uno***

	- GameInitializer --> Se encarga de iniciar el juego.

	- GameController --> Es el que maneja el flujo de juegos, avisa a cada script cuando ejecuta sus funciones (Ej.: Hace que el GameInitializer arranque el mapa y los 	players).

	- TurnManager --> Se encarga de cambiar los turnos y al personaje que le toca jugar.

	- MovementManager --> Calcula los movimientos en la grilla de los personajes y enemigos.

	- Character --> Clase que utlizo para crear a todos los personajes.

	- HealthSystem --> Sistema de vida para todos los personajes.

	- CharacterDataSO --> SO para los datos base de los personajes.

	- GridCell --> Clase usada para crear y guardar información de cada celda en el mapa.

	- ActionsManager --> Maneja las acciones de los personajes.

	- UiCharacterActions --> Avisa cuando se tocan los botones para efectuar las acciones, el juego no es 100% independiente de esta clase (aún siendo UI), por el hecho de 	que un juego de este estilo NECESITA saber cuándo y qué botones de acciones se tocan.

	- UiWinLose --> Muestra en UI cuando se gana y se pierde. El juego es 100% independiente de esta.

	- UiLifeCharacter --> Muestra en UI la vida de cada personaje. El juego es 100% independiente de esta.

- O:
	- Las clases no requieren de modificación para agregar más funcionalidades. La única excepción el GameController, por el hecho de que, al ser una torre central de 	eventos, requeriría conocer las nuevas funcionalidades para enviar eventos o llamar métodos que la utilizen.

- El código repetido se separó en métodos que se utilizan de forma global. Ej.: KillCharacter(List<Character> list, CharacterDataSO data) en GameController.

- Métodos cortos y concisos
