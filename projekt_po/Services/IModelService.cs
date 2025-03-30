namespace projekt_po.Services
{
    public interface IModelService<T> where T : IModelType//interface for model services
    {
        public void Add(T model);
        public bool Delete(int id);
        public bool Update(T model);
        public T? GetById(int id);
        public List<T>? GetAll();
    }

    public interface IModelType
    {
        public int Id { get; set; }
    }
}