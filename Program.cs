using System;
using System.IO;

// CLASE LIBRO
public class Libro
{
    public int Codigo { get; set; }
    public string Titulo { get; set; }
    public string Autor { get; set; }
    public string Categoria { get; set; }
    public int CopiasDisponibles { get; set; }
    public int VecesPrestado { get; set; }

    public Libro(int codigo, string titulo, string autor, string categoria, int copiasDisponibles)
    {
        Codigo = codigo;
        Titulo = titulo;
        Autor = autor;
        Categoria = categoria;
        CopiasDisponibles = copiasDisponibles;
        VecesPrestado = 0;
    }
}

// NODO B+
public class NodoBPlus
{
    public bool Hoja { get; set; }
    public int NumClaves { get; set; }
    public int[] Claves { get; set; }
    public Libro?[] Libros { get; set; }
    public NodoBPlus?[] Hijos { get; set; }
    public NodoBPlus? Siguiente { get; set; }

    public NodoBPlus(int orden, bool hoja = true)
    {
        Hoja = hoja;
        NumClaves = 0;
        Claves = new int[orden];
        Libros = new Libro?[orden];
        Hijos = new NodoBPlus?[orden + 1];
        Siguiente = null;
    }
}

// ARBOL B+
public class ArbolBPlus
{
    private int orden;
    private int maxClaves;
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
        this.raiz = new NodoBPlus(orden, hoja: true);
    }

    // INSERTAR
    public void Insertar(Libro libro)
    {
        if (Buscar(libro.Codigo) != null) return;

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
        MostrarLibrosRecursivo(raiz);
    }

    private void MostrarLibrosRecursivo(NodoBPlus nodo)
    {
        if (nodo.Hoja)
        {
            for (int i = 0; i < nodo.NumClaves; i++)
            {
                Libro? libro = nodo.Libros[i];
                if (libro != null)
                {
                    Console.WriteLine($"Código: {libro.Codigo} | Título: {libro.Titulo} | Autor: {libro.Autor} | Categoría: {libro.Categoria} | Copias: {libro.CopiasDisponibles}");
                }
            }
            return;
        }

        for (int i = 0; i <= nodo.NumClaves; i++)
        {
            if (nodo.Hijos[i] != null) MostrarLibrosRecursivo(nodo.Hijos[i]!);
        }
    }

    //  PERSISTENCIA EN CSV  
    private NodoBPlus PrimeraHoja()
    {
        NodoBPlus actual = raiz;
        while (!actual.Hoja) actual = actual.Hijos[0]!;
        return actual;
    }

    // GUARDAR CSV
    public void GuardarCSV(string rutaArchivo)
    {
        using (StreamWriter sw = new StreamWriter(rutaArchivo, false))
        {
            sw.WriteLine("Codigo,Titulo,Autor,Categoria,CopiasDisponibles,VecesPrestado");

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

        Console.WriteLine($"Datos guardados en '{rutaArchivo}'.");
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

    // CARGAR CSV
    public void CargarCSV(string rutaArchivo)
    {
        if (!File.Exists(rutaArchivo))
        {
            Console.WriteLine($"No se encontró '{rutaArchivo}'. Se inicia con árbol vacío.");
            return;
        }

        using (StreamReader sr = new StreamReader(rutaArchivo))
        {
            string? linea = sr.ReadLine(); // saltar encabezado
            int contador = 0;

            while ((linea = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;

                string[] campos = ParsearLineaCsv(linea);
                if (campos.Length < 6) continue;

                int codigo = int.Parse(campos[0]);
                string titulo = campos[1];
                string autor = campos[2];
                string categoria = campos[3];
                int copias = int.Parse(campos[4]);
                int vecesPrestado = int.Parse(campos[5]);

                Libro libro = new Libro(codigo, titulo, autor, categoria, copias);
                libro.VecesPrestado = vecesPrestado;

                Insertar(libro);
                contador++;
            }

            Console.WriteLine($"Se cargaron {contador} libros desde '{rutaArchivo}'.");
        }
    }

    private const int NUM_COLUMNAS_CSV = 6;

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