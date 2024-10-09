using Grpc.Core;
using ServerApp.Data;
using ServerApp.Models;
using GrpcDocumentService;
using Microsoft.EntityFrameworkCore;
using GrpcDocument = GrpcDocumentService.Document;
using TaskDocument = ServerApp.Models.Document;
using GrpcStatus = Grpc.Core.Status;
using Google.Protobuf.WellKnownTypes;
using ServerApp.Enums;


namespace ServerApp.Services;

public class DocumentService : DocumentGrpc.DocumentGrpcBase
{
    private readonly AppDbContext _dbContext;

    public DocumentService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<GetAllDocumentsResponse> GetAllDocuments(Empty request, ServerCallContext context)
    {
        var documentsQuery = from d in _dbContext.Documents
                             from s1 in _dbContext.DocumentStatuses.Where(ds => d.Id == ds.DocumentId).DefaultIfEmpty()
                             from s2 in _dbContext.DocumentStatuses.Where(ds => d.Id == ds.DocumentId && s1.Id < ds.Id).DefaultIfEmpty()
                             join s in _dbContext.Statuses on s1.StatusId equals s.Id
                             where s2 == null && s1.StatusId != (int)StatusEnum.Deleted
                             select new GrpcDocument
                             {
                                Id = d.Id,
                                Amount = d.Amount,
                                Description = d.Description,
                                Status = s.Name.ToString(),
                                CreatedAt = $"{s1.CreatedAt:yyyy-MM-dd HH:mm:ss}"
                             };
        var documents = await documentsQuery.ToListAsync();

        var response = new GetAllDocumentsResponse();
        response.Documents.AddRange(documents);

        return response;
    }

    public override async Task<GetDocumentByIdResponse> GetDocumentById(GetDocumentByIdRequest request, ServerCallContext context)
    {
        var documentsQuery = from d in _dbContext.Documents
                             from s1 in _dbContext.DocumentStatuses.Where(ds => d.Id == ds.DocumentId).DefaultIfEmpty()
                             from s2 in _dbContext.DocumentStatuses.Where(ds => d.Id == ds.DocumentId && s1.Id < ds.Id).DefaultIfEmpty()
                             join s in _dbContext.Statuses on s1.StatusId equals s.Id
                             where s2 == null && d.Id == request.Id && s1.StatusId != (int)StatusEnum.Deleted
                             select new GrpcDocument
                             {
                                 Id = d.Id,
                                 Amount = d.Amount,
                                 Description = d.Description,
                                 Status = s.Name.ToString(),
                                 CreatedAt = $"{s1.CreatedAt:yyyy-MM-dd HH:mm:ss}"
                             };
        var document = await documentsQuery.FirstOrDefaultAsync();
        if (document == null) 
            throw new RpcException(new GrpcStatus(StatusCode.NotFound, $"Document with id {request.Id} not found"));
            
        var response = new GetDocumentByIdResponse();
        response.Document = document;

        return response;
    }

    public override async Task<CreateDocumentResponse> CreateDocument(CreateDocumentRequest request, ServerCallContext context)
    {
        var createdStatus = StatusEnum.Created;

        var newDocument = new TaskDocument
        {
            Amount = request.Amount,
            Description = request.Description,
            DocumentStatuses =
            [
                new DocumentStatus
                {
                    StatusId = (int)createdStatus,
                    CreatedAt = DateTime.UtcNow
                }
            ]
        };

        _dbContext.Documents.Add(newDocument);
        await _dbContext.SaveChangesAsync();

        return new CreateDocumentResponse
        {
            Id = newDocument.Id,
            StatusId = (int)createdStatus
        };
    }

    public override async Task<Empty> DeleteDocument(DeleteDocumentRequest request, ServerCallContext context)
    {
        var deletedStatus = StatusEnum.Deleted;

        var deletedItem = await (from item in _dbContext.DocumentStatuses
                                 where item.DocumentId == request.Id && item.StatusId != (int)deletedStatus
                                 select item).ExecuteUpdateAsync(d => d.SetProperty(p => p.StatusId, (int)deletedStatus));

        if (deletedItem == null)
            throw new RpcException(new GrpcStatus(StatusCode.NotFound, $"Document with id {request.Id} not found"));

        await _dbContext.SaveChangesAsync();

        return new Empty();
    }        
}

