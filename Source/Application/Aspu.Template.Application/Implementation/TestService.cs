using System.Collections.Concurrent;

namespace Aspu.Template.Application.Implementation;

public interface IScrutorScopedService { }

public interface IService : IScrutorScopedService
{
    string GetValue();
}

public interface IInternalService
{
    string GetInternalValue();
}

public interface IInternal2Service : IService
{
    string GetInternal2Value();
}

public class Service : IService
{
    public string GetValue() => "Service";
}

public class Service1 : IService, IInternalService
{
    public string GetValue() => "Service1";
    public string GetInternalValue() => "Service1";
}

public class Service2 : IService
{
    public string GetValue() => "Service2";
}

public class Service3 : IInternalService, IScrutorScopedService
{
    public string GetInternalValue() => "Service3";
}

public class Service4 : IInternal2Service
{
    public string GetValue() => "Service4";
    public string GetInternal2Value() => "Service4";
}

public interface IFoo { string GetFooValue(); }
public interface IBar { string GetBarValue(); }
public class Foo : IFoo, IBar
{

    public string GetFooValue() => "FooValue";
    public string GetBarValue() => "BarValue";
}

public interface IAuthorRepository
{
    string GetById(Guid id);
}

public class AuthorRepository : IAuthorRepository
{
    public string GetById(Guid id) => "AuthorRepository";
}

public class CachedAuthorRepository(IAuthorRepository repo) : IAuthorRepository
{
    private readonly IAuthorRepository _repo = repo;
    private readonly ConcurrentDictionary<Guid, string> _dict = new();

    public string GetById(Guid id) => _dict.GetOrAdd(id, i => _repo.GetById(i));
}