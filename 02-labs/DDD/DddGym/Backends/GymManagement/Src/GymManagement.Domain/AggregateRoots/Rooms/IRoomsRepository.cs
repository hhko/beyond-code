﻿using LanguageExt;

namespace GymManagement.Domain.AggregateRoots.Rooms;

public interface IRoomsRepository
{
    Task AddRoomAsync(Room room);
    //Task<Room?> GetByIdAsync(Guid id);
    Task<Fin<Room>> GetByIdAsync(Guid id);
    Task<List<Room>> ListByGymIdAsync(Guid gymId);
    Task RemoveAsync(Room room);
    Task UpdateAsync(Room room);
}
