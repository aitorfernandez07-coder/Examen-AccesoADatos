string ruta = "";
string opcion = "1";

while (opcion != "2")
{
    Console.Write("Indique la ruta del CSV con el que quiere trabajar: ");

    ruta = Console.ReadLine() ?? string.Empty;

    if (!ComprobarRutaCSV(ruta))
    {
        Console.WriteLine("La ruta del CSV no es valida.");
        Console.WriteLine();
        Console.WriteLine("¿Prefiere indicar otra ruta o salir del programa?");
        Console.WriteLine("1. Indicar otra ruta");
        Console.WriteLine("2. Salir");
        Console.Write("Seleccione una opción: ");

        opcion = Console.ReadLine() ?? string.Empty;
    } else
    {
        Console.WriteLine();
        Console.WriteLine("Ruta encontrada correctamente.");
        Console.WriteLine("Entrando al menú...");

        opcion = "2";
    }

    while (opcion != "1" && opcion != "2")
    {
        Console.WriteLine();
        Console.WriteLine("Opción no válida.");
        Console.Write("Seleccione 1 para indicar otra ruta o 2 para salir: ");

        opcion = Console.ReadLine() ?? string.Empty;
    }

    Console.WriteLine();
}

if (!ComprobarRutaCSV(ruta))
{
    Console.WriteLine("Saliendo...");

    opcion = "6";
} else

while (opcion != "6")
{
    string[] lineas = LeerLineasCSV(ruta);

    Console.WriteLine("===== ACB 25-26 =====");
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

bool ComprobarRutaCSV (string ruta)
{
    if (ruta == string.Empty)
    {
        return false;
    }
    else if (!ruta.EndsWith(".csv"))
    {
        return false;
    }
    else if (!File.Exists(ruta))
    {
        return false;
    }
    else
    {
        return true;
    }
}

string[] LeerLineasCSV (string ruta)
{
    string[] lineas = File.ReadAllLines(ruta);

    return lineas;
}

string[] SepararCamposCSV (string linea)
{
    string[] datos = linea.Split(",");

    return datos;
}

List<string> ObtenerEquipos(string[] lineas)
{
    List<string> equipos = new List<string>();

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

    return equipos;
}

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

void ObtenerMVPs(string[] lineas, List<string> jugadores, List<int> apariciones)
{
    for (int i = 1; i < lineas.Length; i++)
    {
        string[] datos = SepararCamposCSV(lineas[i]);

        string jugador = datos[4];

        int posicion = jugadores.IndexOf(jugador);

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

void ObtenerClasificacion(string[] lineas, List<string> equipos,
    List<int> victorias, List<int> derrotas)
{
    for (int i = 0; i < equipos.Count; i++)
    {
        victorias.Add(0);
        derrotas.Add(0);
    }

    for (int i = 1; i < lineas.Length; i++)
    {
        string[] datos = SepararCamposCSV(lineas[i]);

        string equipoLocal = datos[1];
        string equipoVisitante = datos[2];

        string[] marcador = datos[3].Split("-");

        int puntosLocal = int.Parse(marcador[0]);
        int puntosVisitante = int.Parse(marcador[1]);

        int posicionLocal = equipos.IndexOf(equipoLocal);
        int posicionVisitante = equipos.IndexOf(equipoVisitante);

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

void GuardarEnFichero(List<string> resultados)
{
    Console.Write("Indique el nombre del fichero: ");
    string nombreFichero = Console.ReadLine() ?? string.Empty;

    if (nombreFichero == string.Empty)
    {
        Console.WriteLine("El nombre del fichero no puede estar vacío.");
        return;
    }

    if (!nombreFichero.EndsWith(".txt"))
    {
        nombreFichero += ".txt";
    }

    File.WriteAllLines(nombreFichero, resultados);

    Console.WriteLine();
    Console.WriteLine($"Fichero {nombreFichero} creado correctamente.");
}

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

            Console.WriteLine();
            Console.WriteLine("¿Quiere guardar los resultados en un fichero?");
            Console.WriteLine("1. Sí");
            Console.WriteLine("2. No");

            string guardar = Console.ReadLine() ?? string.Empty;

            if (guardar == "1")
            {
                GuardarEnFichero(resultados);
            }

            break;
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("Opción no válida. Inténtelo de nuevo.");
            Console.WriteLine();
        }
    }
}

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

            Console.WriteLine();
            Console.WriteLine("¿Quiere guardar los resultados en un fichero?");
            Console.WriteLine("1. Sí");
            Console.WriteLine("2. No");

            string guardar = Console.ReadLine() ?? string.Empty;

            if (guardar == "1")
            {
                GuardarEnFichero(resultados);
            }

            break;
        }
        else
        {
            Console.WriteLine("Opción no válida.");
        }
    }
}

void MostrarTop3MVP(string[] lineas)
{
    List<string> jugadores = new List<string>();
    List<int> apariciones = new List<int>();

    ObtenerMVPs(lineas, jugadores, apariciones);

    List<string> resultados = new List<string>();


    Console.WriteLine("===== TOP 3 MVP =====");
    Console.WriteLine();

    for (int puesto = 1; puesto <= 3; puesto++)
    {
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

        apariciones[posicionMayor] = -1;
    }

    Console.WriteLine();
    Console.WriteLine("¿Quiere guardar los resultados en un fichero?");
    Console.WriteLine("1. Sí");
    Console.WriteLine("2. No");
    
    string guardar = Console.ReadLine() ?? string.Empty;

    if (guardar == "1")
    {
        GuardarEnFichero(resultados);
    }
}



void MostrarClasificacion(string[] lineas)
{
    List<string> equipos = ObtenerEquipos(lineas);
    List<int> victorias = new List<int>();
    List<int> derrotas = new List<int>();

    ObtenerClasificacion(lineas, equipos, victorias, derrotas);

    Console.WriteLine("===== CLASIFICACIÓN =====");
    Console.WriteLine();

    for (int puesto = 1; puesto <= equipos.Count; puesto++)
    {
        int mayor = -1;
        int posicionMayor = -1;

        for (int i = 0; i < victorias.Count; i++)
        {
            if (victorias[i] > mayor)
            {
                mayor = victorias[i];
                posicionMayor = i;
            }
        }

        Console.WriteLine(
            $"{puesto}. {equipos[posicionMayor]} - " +
            $"{victorias[posicionMayor]} victorias - " +
            $"{derrotas[posicionMayor]} derrotas"
        );

        victorias[posicionMayor] = -1;
    }
}

void GuardarResultadosTodosEquipos(string[] lineas)
{
    List<string> equipos = ObtenerEquipos(lineas);

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

        string nombreFichero = equipo + ".txt";

        File.WriteAllLines(nombreFichero, resultados);
    }

    Console.WriteLine("Ficheros de todos los equipos creados correctamente.");
}