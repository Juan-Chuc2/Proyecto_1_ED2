using System;
using System.IO;

// ARBOL B+
public class ArbolBPlus
{
    private int orden;
    private int maxClaves;
    private int minClaves;
    private NodoBPlus raiz;

    // RESULTADO DE DIVISIÓN
    private class ResultadoDivision
    {
        public int ClaveGuia { get; set; }
        public NodoBPlus NodoDerecho { get; set; }

        public ResultadoDivision(int claveGuia, NodoBPlus nodoDerecho)
        {
            ClaveGuia = claveGuia;
            NodoDerecho = nodoDerecho;
        }
    }

    // CONSTRUCTOR
    public ArbolBPlus(int orden = 4)
    {
        this.orden = orden;
        this.maxClaves = orden - 1;

        // Mínimo de claves que debe tener un nodo (no raíz) para estar balanceado.
        this.minClaves = (orden + 1) / 2 - 1;
        if (this.minClaves < 1) this.minClaves = 1;

        this.raiz = new NodoBPlus(orden, hoja: true);
    }

    // INSERTAR
    public void Insertar(Libro libro)
    {
        if (Buscar(libro.Codigo) != null)
        {
            Console.WriteLine($"El código {libro.Codigo} ya existe.");
            return;
        }

        ResultadoDivision? resultado = InsertarRecursivo(raiz, libro);

        if (resultado != null)
        {
            NodoBPlus nuevaRaiz = new NodoBPlus(orden, hoja: false);
            nuevaRaiz.Claves[0] = resultado.ClaveGuia;
            nuevaRaiz.Hijos[0] = raiz;
            nuevaRaiz.Hijos[1] = resultado.NodoDerecho;
            nuevaRaiz.NumClaves = 1;
            raiz = nuevaRaiz;
        }
    }

    // INSERTAR RECURSIVO
    private ResultadoDivision? InsertarRecursivo(NodoBPlus nodo, Libro libro)
    {
        if (nodo.Hoja)
        {
            int clave = libro.Codigo;
            int i = nodo.NumClaves - 1;

            while (i >= 0 && nodo.Claves[i] > clave)
            {
                nodo.Claves[i + 1] = nodo.Claves[i];
                nodo.Libros[i + 1] = nodo.Libros[i];
                i--;
            }

            nodo.Claves[i + 1] = clave;
            nodo.Libros[i + 1] = libro;
            nodo.NumClaves++;

            if (nodo.NumClaves <= maxClaves) return null;

            return DividirHoja(nodo);
        }

        int posicion = 0;
        while (posicion < nodo.NumClaves && libro.Codigo >= nodo.Claves[posicion]) posicion++;

        ResultadoDivision? resultado = InsertarRecursivo(nodo.Hijos[posicion]!, libro);

        if (resultado == null) return null;

        for (int i = nodo.NumClaves; i > posicion; i--) nodo.Claves[i] = nodo.Claves[i - 1];
        for (int i = nodo.NumClaves + 1; i > posicion + 1; i--) nodo.Hijos[i] = nodo.Hijos[i - 1];

        nodo.Claves[posicion] = resultado.ClaveGuia;
        nodo.Hijos[posicion + 1] = resultado.NodoDerecho;
        nodo.NumClaves++;

        if (nodo.NumClaves <= maxClaves) return null;

        return DividirInterno(nodo);
    }

    // DIVIDIR HOJA
    private ResultadoDivision DividirHoja(NodoBPlus hoja)
    {
        int punto = hoja.NumClaves / 2;
        NodoBPlus nuevaHoja = new NodoBPlus(orden, hoja: true);

        for (int i = punto; i < hoja.NumClaves; i++)
        {
            nuevaHoja.Claves[i - punto] = hoja.Claves[i];
            nuevaHoja.Libros[i - punto] = hoja.Libros[i];
            nuevaHoja.NumClaves++;
        }

        hoja.NumClaves = punto;
        nuevaHoja.Siguiente = hoja.Siguiente;
        hoja.Siguiente = nuevaHoja;

        return new ResultadoDivision(nuevaHoja.Claves[0], nuevaHoja);
    }

    // DIVIDIR NODO INTERNO
    private ResultadoDivision DividirInterno(NodoBPlus nodo)
    {
        int centro = nodo.NumClaves / 2;
        int claveQueSube = nodo.Claves[centro];
        NodoBPlus nuevoNodo = new NodoBPlus(orden, hoja: false);

        for (int i = centro + 1; i < nodo.NumClaves; i++)
        {
            nuevoNodo.Claves[i - (centro + 1)] = nodo.Claves[i];
            nuevoNodo.NumClaves++;
        }

        for (int i = centro + 1; i <= nodo.NumClaves; i++)
        {
            nuevoNodo.Hijos[i - (centro + 1)] = nodo.Hijos[i];
        }

        nodo.NumClaves = centro;

        return new ResultadoDivision(claveQueSube, nuevoNodo);
    }

    // BUSCAR
    public Libro? Buscar(int codigo)
    {
        return BuscarRecursivo(raiz, codigo);
    }

    private Libro? BuscarRecursivo(NodoBPlus nodo, int codigo)
    {
        if (nodo.Hoja)
        {
            for (int i = 0; i < nodo.NumClaves; i++)
            {
                if (nodo.Claves[i] == codigo) return nodo.Libros[i];
            }
            return null;
        }

        int posicion = 0;
        while (posicion < nodo.NumClaves && codigo >= nodo.Claves[posicion]) posicion++;

        return BuscarRecursivo(nodo.Hijos[posicion]!, codigo);
    }

    // ELIMINAR
    public bool Eliminar(int codigo)
    {
        if (Buscar(codigo) == null)
        {
            Console.WriteLine($"El código {codigo} no existe.");
            return false;
        }

        EliminarRecursivo(raiz, codigo);

        // Si la raíz quedó vacía (sin claves), su único hijo pasa a ser la nueva raíz.
        if (!raiz.Hoja && raiz.NumClaves == 0) raiz = raiz.Hijos[0]!;

        Console.WriteLine($"Libro con código {codigo} eliminado.");
        return true;
    }

    private void EliminarRecursivo(NodoBPlus nodo, int codigo)
    {
        if (nodo.Hoja)
        {
            EliminarDeHoja(nodo, codigo);
            return;
        }

        int posicion = 0;
        while (posicion < nodo.NumClaves && codigo >= nodo.Claves[posicion]) posicion++;

        NodoBPlus hijo = nodo.Hijos[posicion]!;
        EliminarRecursivo(hijo, codigo);

        if (EstaEnUnderflow(hijo)) CorregirUnderflow(nodo, posicion);
    }

    private void EliminarDeHoja(NodoBPlus hoja, int codigo)
    {
        int indice = -1;
        for (int i = 0; i < hoja.NumClaves; i++)
        {
            if (hoja.Claves[i] == codigo)
            {
                indice = i;
                break;
            }
        }

        if (indice == -1) return;

        for (int i = indice; i < hoja.NumClaves - 1; i++)
        {
            hoja.Claves[i] = hoja.Claves[i + 1];
            hoja.Libros[i] = hoja.Libros[i + 1];
        }

        hoja.Claves[hoja.NumClaves - 1] = 0;
        hoja.Libros[hoja.NumClaves - 1] = null;
        hoja.NumClaves--;
    }

    private bool EstaEnUnderflow(NodoBPlus nodo)
    {
        return nodo.NumClaves < minClaves;
    }

    private void CorregirUnderflow(NodoBPlus padre, int posicion)
    {
        NodoBPlus hijo = padre.Hijos[posicion]!;
        NodoBPlus? hermanoIzquierdo = posicion > 0 ? padre.Hijos[posicion - 1] : null;
        NodoBPlus? hermanoDerecho = posicion < padre.NumClaves ? padre.Hijos[posicion + 1] : null;

        if (hijo.Hoja)
        {
            if (hermanoIzquierdo != null && hermanoIzquierdo.NumClaves > minClaves)
            {
                PrestarDeIzquierdaHoja(padre, posicion);
                return;
            }

            if (hermanoDerecho != null && hermanoDerecho.NumClaves > minClaves)
            {
                PrestarDeDerechaHoja(padre, posicion);
                return;
            }

            if (hermanoIzquierdo != null) FusionarHojas(padre, posicion - 1);
            else FusionarHojas(padre, posicion);
        }
        else
        {
            if (hermanoIzquierdo != null && hermanoIzquierdo.NumClaves > minClaves)
            {
                PrestarDeIzquierdaInterno(padre, posicion);
                return;
            }

            if (hermanoDerecho != null && hermanoDerecho.NumClaves > minClaves)
            {
                PrestarDeDerechaInterno(padre, posicion);
                return;
            }

            if (hermanoIzquierdo != null) FusionarInternos(padre, posicion - 1);
            else FusionarInternos(padre, posicion);
        }
    }

    private void PrestarDeIzquierdaHoja(NodoBPlus padre, int posicion)
    {
        NodoBPlus hijo = padre.Hijos[posicion]!;
        NodoBPlus hermanoIzquierdo = padre.Hijos[posicion - 1]!;

        for (int i = hijo.NumClaves; i > 0; i--)
        {
            hijo.Claves[i] = hijo.Claves[i - 1];
            hijo.Libros[i] = hijo.Libros[i - 1];
        }

        int ultimo = hermanoIzquierdo.NumClaves - 1;
        hijo.Claves[0] = hermanoIzquierdo.Claves[ultimo];
        hijo.Libros[0] = hermanoIzquierdo.Libros[ultimo];

        hermanoIzquierdo.Claves[ultimo] = 0;
        hermanoIzquierdo.Libros[ultimo] = null;

        hijo.NumClaves++;
        hermanoIzquierdo.NumClaves--;
        padre.Claves[posicion - 1] = hijo.Claves[0];
    }

    private void PrestarDeDerechaHoja(NodoBPlus padre, int posicion)
    {
        NodoBPlus hijo = padre.Hijos[posicion]!;
        NodoBPlus hermanoDerecho = padre.Hijos[posicion + 1]!;

        hijo.Claves[hijo.NumClaves] = hermanoDerecho.Claves[0];
        hijo.Libros[hijo.NumClaves] = hermanoDerecho.Libros[0];
        hijo.NumClaves++;

        for (int i = 0; i < hermanoDerecho.NumClaves - 1; i++)
        {
            hermanoDerecho.Claves[i] = hermanoDerecho.Claves[i + 1];
            hermanoDerecho.Libros[i] = hermanoDerecho.Libros[i + 1];
        }

        hermanoDerecho.Claves[hermanoDerecho.NumClaves - 1] = 0;
        hermanoDerecho.Libros[hermanoDerecho.NumClaves - 1] = null;
        hermanoDerecho.NumClaves--;
        padre.Claves[posicion] = hermanoDerecho.Claves[0];
    }

    private void FusionarHojas(NodoBPlus padre, int indiceIzquierdo)
    {
        NodoBPlus izquierdo = padre.Hijos[indiceIzquierdo]!;
        NodoBPlus derecho = padre.Hijos[indiceIzquierdo + 1]!;

        for (int i = 0; i < derecho.NumClaves; i++)
        {
            izquierdo.Claves[izquierdo.NumClaves + i] = derecho.Claves[i];
            izquierdo.Libros[izquierdo.NumClaves + i] = derecho.Libros[i];
        }

        izquierdo.NumClaves += derecho.NumClaves;
        izquierdo.Siguiente = derecho.Siguiente;

        for (int i = indiceIzquierdo; i < padre.NumClaves - 1; i++)
        {
            padre.Claves[i] = padre.Claves[i + 1];
        }

        for (int i = indiceIzquierdo + 1; i < padre.NumClaves; i++)
        {
            padre.Hijos[i] = padre.Hijos[i + 1];
        }

        padre.Hijos[padre.NumClaves] = null;
        padre.NumClaves--;
    }

    private void PrestarDeIzquierdaInterno(NodoBPlus padre, int posicion)
    {
        NodoBPlus hijo = padre.Hijos[posicion]!;
        NodoBPlus hermanoIzquierdo = padre.Hijos[posicion - 1]!;

        for (int i = hijo.NumClaves; i > 0; i--) hijo.Claves[i] = hijo.Claves[i - 1];
        for (int i = hijo.NumClaves + 1; i > 0; i--) hijo.Hijos[i] = hijo.Hijos[i - 1];

        hijo.Claves[0] = padre.Claves[posicion - 1];
        hijo.Hijos[0] = hermanoIzquierdo.Hijos[hermanoIzquierdo.NumClaves];
        hijo.NumClaves++;

        padre.Claves[posicion - 1] = hermanoIzquierdo.Claves[hermanoIzquierdo.NumClaves - 1];

        hermanoIzquierdo.Hijos[hermanoIzquierdo.NumClaves] = null;
        hermanoIzquierdo.Claves[hermanoIzquierdo.NumClaves - 1] = 0;
        hermanoIzquierdo.NumClaves--;
    }

    private void PrestarDeDerechaInterno(NodoBPlus padre, int posicion)
    {
        NodoBPlus hijo = padre.Hijos[posicion]!;
        NodoBPlus hermanoDerecho = padre.Hijos[posicion + 1]!;

        hijo.Claves[hijo.NumClaves] = padre.Claves[posicion];
        hijo.Hijos[hijo.NumClaves + 1] = hermanoDerecho.Hijos[0];
        hijo.NumClaves++;

        padre.Claves[posicion] = hermanoDerecho.Claves[0];

        for (int i = 0; i < hermanoDerecho.NumClaves - 1; i++) hermanoDerecho.Claves[i] = hermanoDerecho.Claves[i + 1];
        for (int i = 0; i < hermanoDerecho.NumClaves; i++) hermanoDerecho.Hijos[i] = hermanoDerecho.Hijos[i + 1];

        hermanoDerecho.Hijos[hermanoDerecho.NumClaves] = null;
        hermanoDerecho.NumClaves--;
    }

    private void FusionarInternos(NodoBPlus padre, int indiceIzquierdo)
    {
        NodoBPlus izquierdo = padre.Hijos[indiceIzquierdo]!;
        NodoBPlus derecho = padre.Hijos[indiceIzquierdo + 1]!;

        izquierdo.Claves[izquierdo.NumClaves] = padre.Claves[indiceIzquierdo];
        izquierdo.NumClaves++;

        for (int i = 0; i < derecho.NumClaves; i++)
        {
            izquierdo.Claves[izquierdo.NumClaves + i] = derecho.Claves[i];
        }

        for (int i = 0; i <= derecho.NumClaves; i++)
        {
            izquierdo.Hijos[izquierdo.NumClaves + i] = derecho.Hijos[i];
        }

        izquierdo.NumClaves += derecho.NumClaves;

        for (int i = indiceIzquierdo; i < padre.NumClaves - 1; i++) padre.Claves[i] = padre.Claves[i + 1];
        for (int i = indiceIzquierdo + 1; i < padre.NumClaves; i++) padre.Hijos[i] = padre.Hijos[i + 1];

        padre.Hijos[padre.NumClaves] = null;
        padre.NumClaves--;
    }

    // PRESTAR UN LIBRO (baja CopiasDisponibles, sube VecesPrestado)
    public bool Prestar(int codigo)
    {
        Libro? libro = Buscar(codigo);

        if (libro == null)
        {
            Console.WriteLine($"El código {codigo} no existe.");
            return false;
        }

        if (libro.CopiasDisponibles <= 0)
        {
            Console.WriteLine($"No hay copias disponibles de '{libro.Titulo}' (código {libro.Codigo}).");
            return false;
        }

        libro.CopiasDisponibles--;
        libro.VecesPrestado++;

        Console.WriteLine($"Se prestó '{libro.Titulo}'. Copias disponibles: {libro.CopiasDisponibles}. Veces prestado: {libro.VecesPrestado}.");
        return true;
    }

    // DEVOLVER UN LIBRO (sube CopiasDisponibles, nunca más allá de CopiasTotales)
    public bool Devolver(int codigo)
    {
        Libro? libro = Buscar(codigo);

        if (libro == null)
        {
            Console.WriteLine($"El código {codigo} no existe.");
            return false;
        }

        if (libro.CopiasDisponibles >= libro.CopiasTotales)
        {
            Console.WriteLine($"No se puede devolver '{libro.Titulo}': ya están las {libro.CopiasTotales} copias en la biblioteca.");
            return false;
        }

        libro.CopiasDisponibles++;

        Console.WriteLine($"Se devolvió '{libro.Titulo}'. Copias disponibles: {libro.CopiasDisponibles}/{libro.CopiasTotales}.");
        return true;
    }

    // MOSTRAR ARBOL
    public void Mostrar()
    {
        MostrarRecursivo(raiz, 0);
    }

    private void MostrarRecursivo(NodoBPlus nodo, int nivel)
    {
        string espacios = new string(' ', nivel * 4);

        if (nodo.Hoja)
        {
            Console.Write($"{espacios}Hoja: [");
            for (int i = 0; i < nodo.NumClaves; i++)
            {
                Console.Write(nodo.Claves[i]);
                if (i < nodo.NumClaves - 1) Console.Write(", ");
            }
            Console.WriteLine("]");
        }
        else
        {
            Console.Write($"{espacios}Interno: [");
            for (int i = 0; i < nodo.NumClaves; i++)
            {
                Console.Write(nodo.Claves[i]);
                if (i < nodo.NumClaves - 1) Console.Write(", ");
            }
            Console.WriteLine("]");

            for (int i = 0; i <= nodo.NumClaves; i++)
            {
                if (nodo.Hijos[i] != null) MostrarRecursivo(nodo.Hijos[i]!, nivel + 1);
            }
        }
    }

    // MOSTRAR LIBROS
    public void MostrarLibros()
    {
        NodoBPlus? hoja = PrimeraHoja();
        if (hoja == null || hoja.NumClaves == 0)
        {
            Console.WriteLine("No hay libros registrados.");
            return;
        }

        while (hoja != null)
        {
            for (int i = 0; i < hoja.NumClaves; i++)
            {
                Libro? libro = hoja.Libros[i];
                if (libro != null)
                {
                    Console.WriteLine($"Código: {libro.Codigo} | Título: {libro.Titulo} | Autor: {libro.Autor} | Categoría: {libro.Categoria} | Copias: {libro.CopiasDisponibles}/{libro.CopiasTotales} | Prestado: {libro.VecesPrestado} veces");
                }
            }
            hoja = hoja.Siguiente;
        }
    }

// Acceso a monticulos min heap y max heap

    public int ContarLibros()
    {
        return ContarLibrosRecursivo(raiz);
    }

    private int ContarLibrosRecursivo(NodoBPlus nodo)
    {
        if (nodo.Hoja)
        {
            int total = 0;
            for (int i = 0; i < nodo.NumClaves; i++)
            {
                if (nodo.Libros[i] != null) total++;
            }
            return total;
        }

        int suma = 0;
        for (int i = 0; i <= nodo.NumClaves; i++)
        {
            if (nodo.Hijos[i] != null) suma += ContarLibrosRecursivo(nodo.Hijos[i]!);
        }
        return suma;
    }

    public Libro[] ObtenerTodosLosLibros()
    {
        int total = ContarLibros();
        Libro[] arreglo = new Libro[total];
        int indice = 0;
        ObtenerLibrosRecursivo(raiz, arreglo, ref indice);
        return arreglo;
    }

    private void ObtenerLibrosRecursivo(NodoBPlus nodo, Libro[] arreglo, ref int indice)
    {
        if (nodo.Hoja)
        {
            for (int i = 0; i < nodo.NumClaves; i++)
            {
                if (nodo.Libros[i] != null)
                {
                    arreglo[indice] = nodo.Libros[i]!;
                    indice++;
                }
            }
            return;
        }

        for (int i = 0; i <= nodo.NumClaves; i++)
        {
            if (nodo.Hijos[i] != null) ObtenerLibrosRecursivo(nodo.Hijos[i]!, arreglo, ref indice);
        }
    }

    // PERSISTENCIA EN CSV
    private NodoBPlus PrimeraHoja()
    {
        NodoBPlus actual = raiz;
        while (!actual.Hoja) actual = actual.Hijos[0]!;
        return actual;
    }

    public void GuardarCSV(string rutaArchivo)
    {
        using (StreamWriter sw = new StreamWriter(rutaArchivo, false))
        {
            sw.WriteLine("Codigo,Titulo,Autor,Categoria,CopiasDisponibles,CopiasTotales,VecesPrestado");

            NodoBPlus? hoja = PrimeraHoja();
            while (hoja != null)
            {
                for (int i = 0; i < hoja.NumClaves; i++)
                {
                    Libro? libro = hoja.Libros[i];
                    if (libro != null) sw.WriteLine(LibroACsv(libro));
                }
                hoja = hoja.Siguiente;
            }
        }

        Console.WriteLine($"Datos guardados exitosamente en '{rutaArchivo}'.");
    }

    private static string LibroACsv(Libro libro)
    {
        return string.Join(",", new string[]
        {
            libro.Codigo.ToString(),
            EscaparCampoCsv(libro.Titulo),
            EscaparCampoCsv(libro.Autor),
            EscaparCampoCsv(libro.Categoria),
            libro.CopiasDisponibles.ToString(),
            libro.CopiasTotales.ToString(),
            libro.VecesPrestado.ToString()
        });
    }

    private static string EscaparCampoCsv(string campo)
    {
        if (campo.Contains(',') || campo.Contains('"') || campo.Contains('\n'))
        {
            return "\"" + campo.Replace("\"", "\"\"") + "\"";
        }
        return campo;
    }

    public void CargarCSV(string rutaArchivo)
    {
        if (!File.Exists(rutaArchivo))
        {
            Console.WriteLine($"No se encontró el archivo '{rutaArchivo}'. Se inicia con un árbol vacío.");
            return;
        }

        using (StreamReader sr = new StreamReader(rutaArchivo))
        {
            string? linea = sr.ReadLine(); // Saltar encabezado
            int contador = 0;

            while ((linea = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;

                string[] campos = ParsearLineaCsv(linea);
                if (campos.Length < 7) continue;

                int codigo = int.Parse(campos[0]);
                string titulo = campos[1];
                string autor = campos[2];
                string categoria = campos[3];
                int copiasDisponibles = int.Parse(campos[4]);
                int copiasTotales = int.Parse(campos[5]);
                int vecesPrestado = int.Parse(campos[6]);

                Libro libro = new Libro(codigo, titulo, autor, categoria, copiasDisponibles, copiasTotales)
                {
                    VecesPrestado = vecesPrestado
                };

                Insertar(libro);
                contador++;
            }

            Console.WriteLine($"Se cargaron {contador} libros desde '{rutaArchivo}'.");
        }
    }

    private const int NUM_COLUMNAS_CSV = 7;

    private static string[] ParsearLineaCsv(string linea)
    {
        string[] campos = new string[NUM_COLUMNAS_CSV];
        string actual = "";
        bool dentroDeComillas = false;
        int indiceCampo = 0;

        for (int i = 0; i < linea.Length; i++)
        {
            char c = linea[i];

            if (dentroDeComillas)
            {
                if (c == '"')
                {
                    if (i + 1 < linea.Length && linea[i + 1] == '"')
                    {
                        actual += '"';
                        i++;
                    }
                    else
                    {
                        dentroDeComillas = false;
                    }
                }
                else actual += c;
            }
            else
            {
                if (c == '"') dentroDeComillas = true;
                else if (c == ',')
                {
                    if (indiceCampo < NUM_COLUMNAS_CSV) campos[indiceCampo] = actual;
                    indiceCampo++;
                    actual = "";
                }
                else actual += c;
            }
        }

        if (indiceCampo < NUM_COLUMNAS_CSV) campos[indiceCampo] = actual;
        indiceCampo++;

        if (indiceCampo < NUM_COLUMNAS_CSV)
        {
            string[] camposIncompletos = new string[indiceCampo];
            for (int j = 0; j < indiceCampo; j++) camposIncompletos[j] = campos[j];
            return camposIncompletos;
        }

        return campos;
    }
}