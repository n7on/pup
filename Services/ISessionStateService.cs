using System.Collections.Generic;

public interface ISessionStateService<T>
{
    void Save(string name, T value);
    T Get(string name);
    List<T> GetAll();
    bool Remove(string name);
    void Clear();
}