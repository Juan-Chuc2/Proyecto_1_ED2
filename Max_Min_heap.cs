using System;

public static class MonticulosLibros
{
    // ========================  MAX HEAP  ======================
    // Ordena por VecesPrestado (el más prestado queda en la raíz).

    private static void HeapifyAbajoMax(Libro[] arreglo, int tamano, int indice)
    {
        int mayor = indice;
        int izquierdo = 2 * indice + 1;
        int derecho = 2 * indice + 2;

        if (izquierdo < tamano && arreglo[izquierdo].VecesPrestado > arreglo[mayor].VecesPrestado) mayor = izquierdo;
        if (derecho < tamano && arreglo[derecho].VecesPrestado > arreglo[mayor].VecesPrestado) mayor = derecho;

        if (mayor != indice)
        {
            Libro temporal = arreglo[indice];
            arreglo[indice] = arreglo[mayor];
            arreglo[mayor] = temporal;
            HeapifyAbajoMax(arreglo, tamano, mayor);
        }
    }

    private static void HeapifyArribaMax(Libro[] arreglo, int indice)
    {
        while (indice > 0)
        {
            int padre = (indice - 1) / 2;
            if (arreglo[padre].VecesPrestado >= arreglo[indice].VecesPrestado) break;

            Libro temporal = arreglo[indice];
            arreglo[indice] = arreglo[padre];
            arreglo[padre] = temporal;
            indice = padre;
        }
    }

    public static void ConstruirMax(Libro[] arreglo, int tamano)
    {
        int ultimoPadre = (tamano / 2) - 1;
        for (int i = ultimoPadre; i >= 0; i--) HeapifyAbajoMax(arreglo, tamano, i);
    }

    public static void InsertarMax(Libro[] arreglo, ref int tamano, Libro libro)
    {
        arreglo[tamano] = libro;
        HeapifyArribaMax(arreglo, tamano);
        tamano++;
    }

    public static Libro? ExtraerMaximo(Libro[] arreglo, ref int tamano)
    {
        if (tamano == 0) return null;

        Libro maximo = arreglo[0];
        tamano--;
        arreglo[0] = arreglo[tamano];
        arreglo[tamano] = null!;
        HeapifyAbajoMax(arreglo, tamano, 0);

        return maximo;
    }

    // Reporte: de libros más prestados. 
    public static void MostrarmasPrestados(Libro[] libros, int cantidad)
    {
        int tamano = libros.Length;
        Libro[] copia = new Libro[tamano];

        for (int i = 0; i < tamano; i++) copia[i] = libros[i];

        ConstruirMax(copia, tamano);

        int limite = cantidad < tamano ? cantidad : tamano;
        Console.WriteLine($"--- Top {limite} libros más prestados ---");

        for (int i = 1; i <= limite; i++)
        {
            Libro? libro = ExtraerMaximo(copia, ref tamano);
            if (libro != null) Console.WriteLine($"{i}. {libro.Titulo} (código {libro.Codigo}) - {libro.VecesPrestado} préstamos");
        }
    }

    // ========================  MIN HEAP  ======================

    private static void HeapifyAbajoMin(Libro[] arreglo, int tamano, int indice)
    {
        int menor = indice;
        int izquierdo = 2 * indice + 1;
        int derecho = 2 * indice + 2;

        if (izquierdo < tamano && arreglo[izquierdo].CopiasDisponibles < arreglo[menor].CopiasDisponibles) menor = izquierdo;
        if (derecho < tamano && arreglo[derecho].CopiasDisponibles < arreglo[menor].CopiasDisponibles) menor = derecho;

        if (menor != indice)
        {
            Libro temporal = arreglo[indice];
            arreglo[indice] = arreglo[menor];
            arreglo[menor] = temporal;
            HeapifyAbajoMin(arreglo, tamano, menor);
        }
    }

    private static void HeapifyArribaMin(Libro[] arreglo, int indice)
    {
        while (indice > 0)
        {
            int padre = (indice - 1) / 2;
            if (arreglo[padre].CopiasDisponibles <= arreglo[indice].CopiasDisponibles) break;

            Libro temporal = arreglo[indice];
            arreglo[indice] = arreglo[padre];
            arreglo[padre] = temporal;
            indice = padre;
        }
    }

    public static void ConstruirMin(Libro[] arreglo, int tamano)
    {
        int ultimoPadre = (tamano / 2) - 1;
        for (int i = ultimoPadre; i >= 0; i--) HeapifyAbajoMin(arreglo, tamano, i);
    }

    public static void InsertarMin(Libro[] arreglo, ref int tamano, Libro libro)
    {
        arreglo[tamano] = libro;
        HeapifyArribaMin(arreglo, tamano);
        tamano++;
    }

    public static Libro? ExtraerMinimo(Libro[] arreglo, ref int tamano)
    {
        if (tamano == 0) return null;

        Libro minimo = arreglo[0];
        tamano--;
        arreglo[0] = arreglo[tamano];
        arreglo[tamano] = null!;
        HeapifyAbajoMin(arreglo, tamano, 0);

        return minimo;
    }

    // Reporte: de libros con menos copias disponibles.
    public static void Mostrar(Libro[] libros, int cantidad)
    {
        int tamano = libros.Length;
        Libro[] copia = new Libro[tamano];

        for (int i = 0; i < tamano; i++) copia[i] = libros[i];

        ConstruirMin(copia, tamano);

        int limite = cantidad < tamano ? cantidad : tamano;
        Console.WriteLine($"--- {limite} libros con menos copias disponibles ---");

        for (int i = 1; i <= limite; i++)
        {
            Libro? libro = ExtraerMinimo(copia, ref tamano);
            if (libro != null) Console.WriteLine($"{i}. {libro.Titulo} (código {libro.Codigo}) - {libro.CopiasDisponibles}/{libro.CopiasTotales} copias disponibles");
        }
    }
}