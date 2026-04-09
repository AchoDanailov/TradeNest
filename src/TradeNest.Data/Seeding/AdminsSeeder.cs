using System.Reflection;
using System.Text.Json;

using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.Data.Seeding.Dtos;
using TradeNest.Data.Seeding.Interfaces;
using TradeNest.GCommon.Exceptions;
using static TradeNest.GCommon.ErrorMessages;

namespace TradeNest.Data.Seeding;

public class AdminsSeeder : BaseEntitySeeder, IAdminsSeeder
{
    private readonly IUsersRepository _usersRepository;
    private readonly IAdminsRepository _adminsRepository;

    public AdminsSeeder(IUsersRepository usersRepository, IAdminsRepository adminsRepository)
    {
        this._usersRepository = usersRepository;
        this._adminsRepository = adminsRepository;
    }
    
    public override string? PathToFile { get; protected set; }
    
    public override async Task SeedEntityDataAsync(string? pathToFile = null)
    {
        string assemblyDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        this.PathToFile = pathToFile ?? 
                          Path.Combine(assemblyDirPath, "Seeding", "Datasets", "admins.json");
        
        string? dataAsJsonString = await GetSeedDataFromFileAsync();
        if (dataAsJsonString == null)
            throw new ArgumentException(string.Format(FileNotFound, pathToFile));
        
        IEnumerable<AdminImportDto>? adminDtos =
            JsonSerializer.Deserialize<IEnumerable<AdminImportDto>>(dataAsJsonString);
        if (adminDtos == null)
            throw new InvalidOperationException(string.Format(SeedingError, this.GetType().Name));

        IEnumerable<Admin> adminsInDb = (await this._adminsRepository
                .GetAllAsync(queryOptions => queryOptions.AsReadOnly()))
            .ToArray();
        
        ICollection<Admin> adminsToImport = new List<Admin>();
        foreach (AdminImportDto adminDto in adminDtos)
        {
            if (!IsValid(adminDto))
                throw new ArgumentException(string.Format(SeedingError, this.GetType().Name));

            if (adminDto.Id == Guid.Empty || adminDto.UserId == Guid.Empty)
            {
                throw new ArgumentException(string.Format(IdCantBeEmptyMessage, 
                    $"AdminId or {nameof(adminDto.UserId)}"));
            }

            if (adminsInDb.Any(a => a.Id == adminDto.Id))
                continue;

            ApplicationUser? user = await this._usersRepository.FindByIdAsync(adminDto.UserId);
            if (user == null)
            {
                throw new ArgumentException(string.Format(NotFoundMessage,
                    nameof(ApplicationUser), adminDto.UserId));
            }

            bool userIsAlreadyAdmin = adminsInDb.Any(a => a.UserId == user.Id);
            if (userIsAlreadyAdmin)
                throw new InvalidOperationException(UserIsAlreadyAnAdminMessage);

            Admin newAdmin = new Admin()
            {
                Id = adminDto.Id,
                UserId = adminDto.UserId
            };
            adminsToImport.Add(newAdmin);
        }

        if (adminsToImport.Any())
        {
            bool addAdminsResult = await this._adminsRepository.AddRangeAsync(adminsToImport);
            if (!addAdminsResult)
                throw new DataPersistException(nameof(addAdminsResult), this.GetType().Name);
        }
    }
}