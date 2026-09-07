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