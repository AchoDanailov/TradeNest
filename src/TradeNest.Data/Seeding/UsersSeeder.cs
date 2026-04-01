using System.Reflection;
using System.Text.Json;

using static TradeNest.GCommon.ErrorMessages;
using TradeNest.GCommon.Exceptions;
using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.Data.Seeding.Dtos;
using TradeNest.Data.Seeding.Interfaces;

namespace TradeNest.Data.Seeding;

public class UsersSeeder : BaseEntitySeeder, IUsersSeeder
{
    private readonly IUsersRepository _usersRepository;

    public UsersSeeder(IUsersRepository usersRepository)
    {
        this._usersRepository = usersRepository;
    }

    public override string? PathToFile { get; protected set; }

    public override async Task SeedEntityDataAsync(string? pathToFile = null)
    {
        string assemblyDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        this.PathToFile = pathToFile ?? 
                          Path.Combine(assemblyDirPath, "Seeding", "Datasets", "users.json");

        string? dataAsJsonString = await this.GetSeedDataFromFileAsync();
        if (dataAsJsonString == null)
            throw new ArgumentException(string.Format(FileNotFound, pathToFile));

        IEnumerable<UserImportDto>? userDtos =
            JsonSerializer.Deserialize<IEnumerable<UserImportDto>>(dataAsJsonString);
        if (userDtos == null)
            throw new InvalidOperationException(string.Format(SeedingError, this.GetType().Name));

        foreach (UserImportDto userDto in userDtos)
        {
            if (!IsValid(userDto))
                throw new ArgumentException(string.Format(SeedingError, this.GetType().Name));

            if (userDto.Id == Guid.Empty)
            {
                throw new ArgumentException(string.Format(IdCantBeEmptyMessage, 
                    nameof(userDto.Id)));
            }

            bool userExists = await this._usersRepository.ExistsAsync(u => u.Id == userDto.Id);
            if (userExists)
                continue;

            ApplicationUser user = new ApplicationUser()
            {
                Id = userDto.Id,
                UserName = userDto.UserName,
                Email = userDto.Email,
                EmailConfirmed = true,
            };

            bool addUserResult = await this._usersRepository.AddAsync(user, userDto.Password);
            if (!addUserResult)
            {
                throw new DataPersistException(nameof(addUserResult),
                    this.GetType().Name, $"userId: {userDto.Id}");
            }
        }
    }
}