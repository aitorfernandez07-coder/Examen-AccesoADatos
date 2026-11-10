string ruta = "acb2526.csv";

// Se comprueba que el CSV existe para evitar errores al leerlo
if (!ComprobarRutaCSV(ruta))
{
    Console.WriteLine($"No se ha encontrado el fichero {ruta}.");
    Console.WriteLine($"Colóquelo en esta carpeta: {Directory.GetCurrentDirectory()}");
    Console.WriteLine();
    Console.WriteLine("Saliendo...");
}
else
{
    // Se lee el CSV una sola vez y se reutilizan las líneas en todas las opciones
    string[] lineas = LeerLineasCSV(ruta);
    string opcion = "";

    while (opcion != "6")
    {
        Console.WriteLine("ACB 25-26:");
        Console.WriteLine("1. Ver resultados de un equipo");
        Console.WriteLine("2. Ver resultados de una jornada");
        Console.WriteLine("3. Ver Top 3 MVP");
        Console.WriteLine("4. Ver clasificación");
        Console.WriteLine("5. Guardar resultados de todos los equipos");
        Console.WriteLine();
        Console.WriteLine("6. Salir");
        Console.WriteLine();
        Console.Write("Selecciona una opción: ");

        opcion = Console.ReadLine() ?? string.Empty;

        switch (opcion)
        {
            case "1":
                MostrarResultadosEquipo(lineas);

                Console.WriteLine();
                Console.WriteLine("Pulse una tecla para continuar...");
                Console.ReadKey();

                break;

            case "2":
                MostrarResultadosJornada(lineas);

                Console.WriteLine();
                Console.WriteLine("Pulse una tecla para continuar...");
                Console.ReadKey();

                break;

            case "3":
                MostrarTop3MVP(lineas);

                Console.WriteLine();
                Console.WriteLine("Pulse una tecla para continuar...");
                Console.ReadKey();

                break;

            case "4":
                MostrarClasificacion(lineas);

                Console.WriteLine();
                Console.WriteLine("Pulse una tecla para continuar...");
                Console.ReadKey();

                break;

            case "5":
                GuardarResultadosTodosEquipos(lineas);

                Console.WriteLine();
                Console.WriteLine("Pulse una tecla para continuar...");
                Console.ReadKey();

                break;

            case "6":
                Console.WriteLine("Saliendo...");
                break;

            default:
                Console.WriteLine("Opción no válida.");
                break;
        }

        Console.WriteLine();
    }
}

// FUNCIONES
// Devuelve true si el fichero CSV existe en la ruta indicada
bool ComprobarRutaCSV(string ruta)
{
    return File.Exists(ruta);
}

// Lee todas las líneas del CSV (la primera línea es la cabecera)
string[] LeerLineasCSV(string ruta)
{
    string[] lineas = File.ReadAllLines(ruta);

    return lineas;
}

// Separa una línea del CSV en sus campos (están separados por comas)
string[] SepararCamposCSV(string linea)
{
    string[] datos = linea.Split(",");

    return datos;
}

// Devuelve la lista de equipos sin repetir, ordenada alfabéticamente
List<string> ObtenerEquipos(string[] lineas)
{
    List<string> equipos = new List<string>();

    // Se empieza en 1 para saltarse la cabecera
    for (int i = 1; i < lineas.Length; i++)
    {
        string[] datos = SepararCamposCSV(lineas[i]);

        if (!equipos.Contains(datos[1]))
        {
            equipos.Add(datos[1]);
        }

        if (!equipos.Contains(datos[2]))
        {
            equipos.Add(datos[2]);
        }
    }

    equipos.Sort();

    return equipos;
}

// Devuelve la lista de jornadas sin repetir (en el orden en que aparecen en el CSV)
List<string> ObtenerJornadas(string[] lineas)
{
    List<string> jornadas = new List<string>();

    for (int i = 1; i < lineas.Length; i++)
    {
        string[] datos = SepararCamposCSV(lineas[i]);

        if (!jornadas.Contains(datos[0]))
        {
            jornadas.Add(datos[0]);
        }
    }

    return jornadas;
}

// Rellena dos listas paralelas: el jugador y las veces que ha sido MVP
void ObtenerMVPs(string[] lineas, List<string> jugadores, List<int> apariciones)
{
    for (int i = 1; i < lineas.Length; i++)
    {
        string[] datos = SepararCamposCSV(lineas[i]);

        string jugador = datos[4];

        int posicion = jugadores.IndexOf(jugador);

        // Si el jugador no estaba en la lista se añade con 1 aparición, y si ya estaba se le suma una
        if (posicion == -1)
        {
            jugadores.Add(jugador);
            apariciones.Add(1);
        }
        else
        {
            apariciones[posicion]++;
        }
    }
}

// Rellena las listas paralelas de victorias, derrotas y diferencia de puntos de cada equipo (la posición coincide con la de la lista equipos)
void ObtenerClasificacion(string[] lineas, List<string> equipos, List<int> victorias, List<int> derrotas, List<int> diferencias)
{
    // Todos los equipos empiezan con 0
    for (int i = 0; i < equipos.Count; i++)
    {
        victorias.Add(0);
        derrotas.Add(0);
        diferencias.Add(0);
    }

    for (int i = 1; i < lineas.Length; i++)
    {
        string[] datos = SepararCamposCSV(lineas[i]);

        string equipoLocal = datos[1];
        string equipoVisitante = datos[2];

        // El resultado viene primero local luego visitante, separados por un -
        string[] marcador = datos[3].Split("-");

        int puntosLocal = int.Parse(marcador[0]);
        int puntosVisitante = int.Parse(marcador[1]);

        int posicionLocal = equipos.IndexOf(equipoLocal);
        int posicionVisitante = equipos.IndexOf(equipoVisitante);

        // La diferencia de puntos se usa después para desempatar
        diferencias[posicionLocal] += puntosLocal - puntosVisitante;
        diferencias[posicionVisitante] += puntosVisitante - puntosLocal;

        // En baloncesto no hay empates, así que siempre hay un ganador
        if (puntosLocal > puntosVisitante)
        {
            victorias[posicionLocal]++;
            derrotas[posicionVisitante]++;
        }
        else
        {
            victorias[posicionVisitante]++;
            derrotas[posicionLocal]++;
        }
    }
}

// Pregunta al usuario en qué carpeta quiere guardar los ficheros.
// Si no pones nada se usa la carpeta actual. Si la carpeta no existe, vuelve a preguntar.
string PedirCarpetaDestino()
{
    string carpeta = "";
    bool carpetaValida = false;

    while (!carpetaValida)
    {
        Console.Write("Indique la carpeta donde guardar (si no pones nada se guarda en la carpeta actual): ");

        // Se quitan espacios y comillas (se ponen por defecto al poner una ruta)
        carpeta = (Console.ReadLine() ?? string.Empty).Trim().Trim('"');

        if (carpeta == string.Empty)
        {
            carpeta = Directory.GetCurrentDirectory();
            carpetaValida = true;
        }
        else if (Directory.Exists(carpeta))
        {
            carpetaValida = true;
        }
        else
        {
            Console.WriteLine("La carpeta no existe. Inténtelo de nuevo.");
            Console.WriteLine();
        }
    }

    return carpeta;
}

// Pide el nombre del fichero y la carpeta, y guarda ahí los resultados en un .txt
void GuardarEnFichero(List<string> resultados)
{
    Console.Write("Indique el nombre del fichero: ");
    string nombreFichero = (Console.ReadLine() ?? string.Empty).Trim();

    if (nombreFichero == string.Empty)
    {
        Console.WriteLine("El nombre del fichero no puede estar vacío.");
        return;
    }

    // Evita errores por caracteres que no se pueden usar en un nombre de fichero
    if (nombreFichero.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
    {
        Console.WriteLine("El nombre del fichero contiene caracteres no válidos.");
        return;
    }

    if (!nombreFichero.EndsWith(".txt"))
    {
        nombreFichero += ".txt";
    }

    string carpeta = PedirCarpetaDestino();

    // Path.Combine une la carpeta y el nombre con el separador correcto
    string rutaFichero = Path.Combine(carpeta, nombreFichero);

    File.WriteAllLines(rutaFichero, resultados);

    Console.WriteLine();
    Console.WriteLine($"Fichero creado correctamente en: {Path.GetFullPath(rutaFichero)}");
}

// Pregunta si se quieren guardar los resultados y, si es que sí, los guarda.
// Se repite hasta que el usuario responda 1 o 2.
void PreguntarSiGuardar(List<string> resultados)
{
    string guardar = "";

    while (guardar != "1" && guardar != "2")
    {
        Console.WriteLine();
        Console.WriteLine("¿Quiere guardar los resultados en un fichero?");
        Console.WriteLine("1. Sí");
        Console.WriteLine("2. No");
        Console.Write("Seleccione una opción: ");

        guardar = Console.ReadLine() ?? string.Empty;

        if (guardar != "1" && guardar != "2")
        {
            Console.WriteLine("Opción no válida.");
        }
    }

    if (guardar == "1")
    {
        GuardarEnFichero(resultados);
    }
}

// Muestra la lista de equipos, deja elegir uno y enseña todos sus partidos
void MostrarResultadosEquipo(string[] lineas)
{
    List<string> equipos = ObtenerEquipos(lineas);

    int opcionEquipo = -1;

    while (opcionEquipo != 0)
    {
        Console.WriteLine("Equipos disponibles:");
        Console.WriteLine();

        for (int i = 0; i < equipos.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {equipos[i]}");
        }

        Console.WriteLine("0. Cancelar");
        Console.WriteLine();
        Console.Write("Seleccione una opción: ");

        if ((int.TryParse(Console.ReadLine(), out opcionEquipo)) &&
            (opcionEquipo >= 0) && (opcionEquipo <= equipos.Count))
        {
            if (opcionEquipo == 0)
            {
                Console.WriteLine("Operación cancelada.");
                break;
            }

            string equipoSeleccionado = equipos[opcionEquipo - 1];

            Console.WriteLine();
            Console.WriteLine($"Resultados de {equipoSeleccionado}:");
            Console.WriteLine();

            List<string> resultados = new List<string>();

            for (int i = 1; i < lineas.Length; i++)
            {
                string[] datos = SepararCamposCSV(lineas[i]);

                // Partidos en los que el equipo juega como local o como visitante
                if ((datos[1] == equipoSeleccionado) ||
                    (datos[2] == equipoSeleccionado))
                {
                    string resultado =
                        $"Jornada: {datos[0]}\n" +
                        $"Local: {datos[1]}\n" +
                        $"Visitante: {datos[2]}\n" +
                        $"Resultado: {datos[3]}\n" +
                        $"MVP: {datos[4]}\n" +
                        $"Puntos MVP: {datos[5]}\n" +
                        $"Asistencias MVP: {datos[6]}\n" +
                        $"Rebotes MVP: {datos[7]}\n" +
                        $"Equipo MVP: {datos[8]}\n" +
                        " ";

                    Console.WriteLine(resultado);

                    resultados.Add(resultado);
                }
            }

            PreguntarSiGuardar(resultados);

            break;
        }
        else
        {
            // Si el usuario escribe letras, TryParse deja opcionEquipo en 0 y el while terminaría sin avisar. Se pone en -1 para que vuelva a preguntar.
            opcionEquipo = -1;

            Console.WriteLine();
            Console.WriteLine("Opción no válida. Inténtelo de nuevo.");
            Console.WriteLine();
        }
    }
}

// Muestra la lista de jornadas, deja elegir una y enseña todos sus partidos
void MostrarResultadosJornada(string[] lineas)
{
    List<string> jornadas = ObtenerJornadas(lineas);

    int opcionJornada = -1;

    while (opcionJornada != 0)
    {
        Console.WriteLine("Jornadas disponibles:");
        Console.WriteLine();

        for (int i = 0; i < jornadas.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {jornadas[i]}");
        }

        Console.WriteLine("0. Cancelar");
        Console.WriteLine();
        Console.Write("Seleccione una opción: ");

        if (int.TryParse(Console.ReadLine(), out opcionJornada) &&
            opcionJornada >= 0 &&
            opcionJornada <= jornadas.Count)
        {
            if (opcionJornada == 0)
            {
                Console.WriteLine("Operación cancelada.");
                return;
            }

            string jornadaSeleccionada = jornadas[opcionJornada - 1];

            Console.WriteLine();
            Console.WriteLine($"Resultados de {jornadaSeleccionada}:");
            Console.WriteLine();

            List<string> resultados = new List<string>();

            for (int i = 1; i < lineas.Length; i++)
            {
                string[] datos = SepararCamposCSV(lineas[i]);

                if (datos[0] == jornadaSeleccionada)
                {
                    string resultado =
                        $"Local: {datos[1]}\n" +
                        $"Visitante: {datos[2]}\n" +
                        $"Resultado: {datos[3]}\n" +
                        $"MVP: {datos[4]}\n" +
                        $"Puntos MVP: {datos[5]}\n" +
                        $"Asistencias MVP: {datos[6]}\n" +
                        $"Rebotes MVP: {datos[7]}\n" +
                        $"Equipo MVP: {datos[8]}\n" +
                        " ";

                    Console.WriteLine(resultado);

                    resultados.Add(resultado);
                }
            }

            PreguntarSiGuardar(resultados);

            break;
        }
        else
        {
            // Mismo motivo que en MostrarResultadosEquipo: evitar que unas letras cierren el bucle
            opcionJornada = -1;

            Console.WriteLine();
            Console.WriteLine("Opción no válida. Inténtelo de nuevo.");
            Console.WriteLine();
        }
    }
}

// Muestra los 3 jugadores que más veces han sido MVP
void MostrarTop3MVP(string[] lineas)
{
    List<string> jugadores = new List<string>();
    List<int> apariciones = new List<int>();

    ObtenerMVPs(lineas, jugadores, apariciones);

    List<string> resultados = new List<string>();

    Console.WriteLine("TOP 3 MVP:");
    Console.WriteLine();

    // "puesto <= jugadores.Count" evita errores si hubiera menos de 3 jugadores
    for (int puesto = 1; puesto <= 3 && puesto <= jugadores.Count; puesto++)
    {
        // Se busca el jugador con más apariciones
        int mayor = -1;
        int posicionMayor = -1;

        for (int i = 0; i < apariciones.Count; i++)
        {
            if (apariciones[i] > mayor)
            {
                mayor = apariciones[i];
                posicionMayor = i;
            }
        }

        string resultado = $"{puesto}. {jugadores[posicionMayor]} - {apariciones[posicionMayor]} veces MVP";

        Console.WriteLine(resultado);

        resultados.Add(resultado);

        // Se marca con -1 para que no vuelva a salir en el siguiente puesto
        apariciones[posicionMayor] = -1;
    }

    PreguntarSiGuardar(resultados);
}

// Muestra la clasificación ordenada por victorias.
// Si dos equipos empatan a victorias, desempata la diferencia de puntos.
void MostrarClasificacion(string[] lineas)
{
    List<string> equipos = ObtenerEquipos(lineas);
    List<int> victorias = new List<int>();
    List<int> derrotas = new List<int>();
    List<int> diferencias = new List<int>();

    ObtenerClasificacion(lineas, equipos, victorias, derrotas, diferencias);

    List<string> resultados = new List<string>();

    Console.WriteLine("CLASIFICACIÓN:");
    Console.WriteLine("En caso de empate a victorias desempata la diferencia de puntos.");
    Console.WriteLine();

    for (int puesto = 1; puesto <= equipos.Count; puesto++)
    {
        int mayor = -1;
        int mejorDiferencia = 0;
        int posicionMayor = -1;

        for (int i = 0; i < victorias.Count; i++)
        {
            // Gana el que tenga más victorias; si empatan, el de mejor diferencia de puntos
            if (victorias[i] > mayor ||
                (victorias[i] == mayor && diferencias[i] > mejorDiferencia))
            {
                mayor = victorias[i];
                mejorDiferencia = diferencias[i];
                posicionMayor = i;
            }
        }

        string resultado =
            $"{puesto}. {equipos[posicionMayor]} - " +
            $"{victorias[posicionMayor]} victorias - " +
            $"{derrotas[posicionMayor]} derrotas - " +
            $"dif. de puntos: {diferencias[posicionMayor]}";

        Console.WriteLine(resultado);

        resultados.Add(resultado);

        // Se marca con -1 para que no vuelva a salir en el siguiente puesto
        victorias[posicionMayor] = -1;
    }

    PreguntarSiGuardar(resultados);
}

// Crea un fichero .txt por cada equipo (con su nombre) con todos sus partidos
void GuardarResultadosTodosEquipos(string[] lineas)
{
    List<string> equipos = ObtenerEquipos(lineas);

    // La carpeta se pregunta una sola vez y se usa para todos los ficheros
    string carpeta = PedirCarpetaDestino();

    for (int i = 0; i < equipos.Count; i++)
    {
        string equipo = equipos[i];

        List<string> resultados = new List<string>();

        for (int j = 1; j < lineas.Length; j++)
        {
            string[] datos = SepararCamposCSV(lineas[j]);

            if (datos[1] == equipo || datos[2] == equipo)
            {
                string resultado =
                    $"Jornada: {datos[0]}\n" +
                    $"Local: {datos[1]}\n" +
                    $"Visitante: {datos[2]}\n" +
                    $"Resultado: {datos[3]}\n" +
                    $"MVP: {datos[4]}\n" +
                    $"Puntos MVP: {datos[5]}\n" +
                    $"Asistencias MVP: {datos[6]}\n" +
                    $"Rebotes MVP: {datos[7]}\n" +
                    $"Equipo MVP: {datos[8]}\n" +
                    " ";

                resultados.Add(resultado);
            }
        }

        string rutaFichero = Path.Combine(carpeta, equipo + ".txt");

        File.WriteAllLines(rutaFichero, resultados);
    }

    Console.WriteLine();
    Console.WriteLine($"{equipos.Count} ficheros creados correctamente en: {Path.GetFullPath(carpeta)}");
}