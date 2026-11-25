namespace Steel.Services
{
    public interface IConfigSource
    {
        string Read(string name);
    }
}
