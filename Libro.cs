using System;

// CLASE LIBRO
public class Libro
{
    public int Codigo { get; set; }
    public string Titulo { get; set; }
    public string Autor { get; set; }
    public string Categoria { get; set; }
    public int CopiasDisponibles { get; set; }
    public int CopiasTotales { get; set; }
    public int VecesPrestado { get; set; }

    public Libro(int codigo, string titulo, string autor, string categoria, int copiasDisponibles, int? copiasTotales = null)
    {
        Codigo = codigo;
        Titulo = titulo;
        Autor = autor;
        Categoria = categoria;
        CopiasDisponibles = copiasDisponibles;
        CopiasTotales = copiasTotales ?? copiasDisponibles;
        VecesPrestado = 0;
    }
}