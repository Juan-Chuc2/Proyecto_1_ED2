﻿using System;

class Program
{
    static ArbolBPlus arbol = new ArbolBPlus(4);
    static string rutaCsv = "libros.csv";

    static void Main()
    {
        arbol.CargarCSV(rutaCsv); // cargar al iniciar

        bool salir = false;
        while (!salir)
        {
            Console.WriteLine("\n--- MENÚ BIBLIOTECA ---");
            Console.WriteLine("1. Insertar libro");
            Console.WriteLine("2. Buscar libro");
            Console.WriteLine("3. Eliminar libro");
            Console.WriteLine("4. Prestar libro");
            Console.WriteLine("5. Devolver libro");
            Console.WriteLine("6. Mostrar todos los libros");
            Console.WriteLine("7. Mostrar estructura del árbol");
            Console.WriteLine("8. Top libros más prestados");
            Console.WriteLine("9. Libros con menos copias disponibles");
            Console.WriteLine("10. Guardar y salir");
            Console.Write("Opción: ");

            string? opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1": InsertarDesdeMenu(); break;
                case "2": BuscarDesdeMenu(); break;
                case "3": EliminarDesdeMenu(); break;
                case "4": PrestarDesdeMenu(); break;
                case "5": DevolverDesdeMenu(); break;
                case "6": arbol.MostrarLibros(); break;
                case "7": arbol.Mostrar(); break;
                case "8": TopPrestadosDesdeMenu(); break;
                case "9": MenosCopiasDesdeMenu(); break;
                case "10":
                    arbol.GuardarCSV(rutaCsv);
                    salir = true;
                    break;
                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }
        }
    }

    static void InsertarDesdeMenu()
    {
        Console.Write("Código: ");
        int codigo = int.Parse(Console.ReadLine()!);
        Console.Write("Título: ");
        string titulo = Console.ReadLine()!;
        Console.Write("Autor: ");
        string autor = Console.ReadLine()!;
        Console.Write("Categoría: ");
        string categoria = Console.ReadLine()!;
        Console.Write("Copias: ");
        int copias = int.Parse(Console.ReadLine()!);

        arbol.Insertar(new Libro(codigo, titulo, autor, categoria, copias));
    }

    static void BuscarDesdeMenu()
    {
        Console.Write("Código a buscar: ");
        int codigo = int.Parse(Console.ReadLine()!);

        Libro? libro = arbol.Buscar(codigo);
        if (libro != null)
            Console.WriteLine($"{libro.Titulo} - {libro.Autor} - Copias: {libro.CopiasDisponibles}/{libro.CopiasTotales}");
        else
            Console.WriteLine("No encontrado.");
    }

    static void EliminarDesdeMenu()
    {
        Console.Write("Código a eliminar: ");
        int codigo = int.Parse(Console.ReadLine()!);
        arbol.Eliminar(codigo);
    }

    static void PrestarDesdeMenu()
    {
        Console.Write("Código a prestar: ");
        int codigo = int.Parse(Console.ReadLine()!);
        arbol.Prestar(codigo);
    }

    static void DevolverDesdeMenu()
    {
        Console.Write("Código a devolver: ");
        int codigo = int.Parse(Console.ReadLine()!);
        arbol.Devolver(codigo);
    }

    // MAX HEAP: en tu archivo el método se llama "MostrarmasPrestados".
    static void TopPrestadosDesdeMenu()
    {
        Console.Write("¿Cuántos libros de los mas pretados quieres ver?: ");
        int cantidad = int.Parse(Console.ReadLine()!);

        Libro[] libros = arbol.ObtenerTodosLosLibros();
        MonticulosLibros.MostrarmasPrestados(libros, cantidad);
    }

    // MIN HEAP: en tu archivo el método se llama "Mostrar" (dentro de MonticulosLibros).
    static void MenosCopiasDesdeMenu()
    {
        Console.Write("¿Cuántos libros quieres ver con menos copias?: ");
        int cantidad = int.Parse(Console.ReadLine()!);

        Libro[] libros = arbol.ObtenerTodosLosLibros();
        MonticulosLibros.Mostrar(libros, cantidad);
    }
}