﻿namespace GymManagement.Application.Usecases.Gyms.Queries.ListSessions;

//// TODO: LanguageExt
//internal sealed class ListSessionsQueryUsecase
//    : IQueryUsecase<ListSessionsQuery, ListSessionsResponse>
//{
//    private readonly ISessionsRepository _sessionsRepository;

//    public ListSessionsQueryUsecase(ISessionsRepository sessionsRepository)
//    {
//        _sessionsRepository = sessionsRepository;
//    }

//    public async Task<IErrorOr<ListSessionsResponse>> Handle(ListSessionsQuery query, CancellationToken cancellationToken)
//    {
//        List<Session> sessions = await _sessionsRepository.ListByGymIdAsync(
//            query.GymId,
//            query.StartDateTime,
//            query.EndDateTime,
//            query.Categories);

//        return sessions
//            .ToResponse()
//            .ToErrorOr();
//    }
//}
