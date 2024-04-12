

using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

SqlRepo<Employee> empRepo = new SqlRepo<Employee>(new EFcoreContext());
AddEmployees(empRepo);
AddManagers(empRepo);
WriteAllToConsole(empRepo);

ListRepo<Orgnization> orgRepo = new ListRepo<Orgnization>();
AddOrgnizations(orgRepo);
WriteAllToConsole(orgRepo);

Console.WriteLine();
;

static void WriteAllToConsole(IReadRepo<IEntityBase> repo)
{
    var items = repo.GetAll();
    foreach (var item in items)
    {
        Console.WriteLine(item);
    }
} // covariance

static void AddManagers(IWriteRepo<Manager> empRepo)
{
    var mgrs = new[]
    {
        new Manager() {EmpName="alizadeh"}
    };

    AddBatch(empRepo, mgrs);
    empRepo.Save();
} // contravariance

static void AddEmployees(IRepo<Employee> empRepo)
{
    var emps = new[]
    {
        new Employee() { EmpName = "ali" }
    };

    AddBatch(empRepo, emps);
    empRepo.Save();
}

static void AddOrgnizations(IRepo<Orgnization> orgRepo)
{
    var orgs = new[]
    {
        new Orgnization() { Name = "manir" }
    };

    AddBatch(orgRepo, orgs);
}

static void AddBatch<T>(IWriteRepo<T> orgRepo, T[] orgs)
{
    foreach (var org in orgs)
    {
        orgRepo.Add(org);
    }
}

//--------------------------------------

public interface IReadRepo<out T>
{
    IEnumerable<T> GetAll();
}

public interface IWriteRepo<in T>
{
    void Add(T item);
    void Save();
}

public interface IRepo<T> : IReadRepo<T>, IWriteRepo<T>
    where T : IEntityBase
{
}

public class ListRepo<T> : IRepo<T>
    where T : IEntityBase
{
    private readonly List<T> items = new();

    public IEnumerable<T> GetAll() => items.ToList();

    public void Add(T item)
    {
        item.Id = items.Any() ? items.Max(x => x.Id) + 1 : 1;
        items.Add(item);
    }

    public void Save()
    {
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }

}

public class SqlRepo<T> : IRepo<T>
    where T : class, IEntityBase
{
    private readonly DbContext dbContext;
    private readonly DbSet<T> dbSet;

    public SqlRepo(DbContext context)
    {
        dbContext = context;
        dbSet = context.Set<T>();
    }

    public IEnumerable<T> GetAll() => dbSet.ToList();

    public void Add(T item)
    {
        item.Id = dbSet.Any() ? dbSet.Max(x => x.Id) + 1 : 1;
        dbSet.Add(item);
    }

    public void Save()
    {
        dbContext.SaveChanges();
    }

}

//--------------------------------------

public class EFcoreContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseInMemoryDatabase("EmpDb");
    }
}

//--------------------------------------

public interface IEntityBase
{
    public int Id { get; set; }
}

public class EntityBase : IEntityBase
{
    public int Id { get; set; }
}

public class Employee : EntityBase
{
    public string EmpName { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"id: {Id}, emp: {EmpName}";
    }
}

public class Manager : Employee
{
    public override string ToString()
    {
        return $"id: {Id}, mgmt: {EmpName}";
    }
}

public class Orgnization : EntityBase
{
    public string Name { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"id: {Id}, org: {Name}";
    }
}