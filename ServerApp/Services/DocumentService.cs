using Grpc.Core;
using ServerApp.Data;
using ServerApp.Models;
using GrpcDocumentService;
using Microsoft.EntityFrameworkCore;
using GrpcDocument = GrpcDocumentService.Document;
using TaskDocument = ServerApp.Models.Document;
using Status = ServerApp.Models.Status;
using GrpcStatus = Grpc.Core.Status;
using Google.Protobuf.WellKnownTypes;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.Intrinsics.Arm;


namespace ServerApp.Services {

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
                                 where s2 == null && s1.StatusId != 2
                                 select new GrpcDocument
                                 {
                                     Id = d.Id,
                                     Amount = d.Amount,
                                     Description = d.Description,
                                     Status = s.Name,
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
                                 where s2 == null && d.Id == request.Id && s1.StatusId != 2
                                 select new GrpcDocument
                                 {
                                     Id = d.Id,
                                     Amount = d.Amount,
                                     Description = d.Description,
                                     Status = s.Name,
                                     CreatedAt = $"{s1.CreatedAt:yyyy-MM-dd HH:mm:ss}"
                                 };
            var document = await documentsQuery.FirstOrDefaultAsync();

            var response = new GetDocumentByIdResponse();
            response.Document = document;

            return response;
        }


        public override async Task<CreateDocumentResponse> CreateDocument(CreateDocumentRequest request, ServerCallContext context)
        {
            var createdStatus = from s in _dbContext.Statuses
                                  where s.Id == 1
                                  select s;

            var newDocument = new TaskDocument
            {
                Amount = request.Amount,
                Description = request.Description,
                DocumentStatuses =
                [
                    new DocumentStatus
                    {

                        StatusId = createdStatus.FirstOrDefault().Id,
                        CreatedAt = DateTime.UtcNow,
                        Status = createdStatus.FirstOrDefault()
                        

                    }
                ]

            };

            _dbContext.Documents.Add(newDocument);
            _dbContext.DocumentStatuses.Add(newDocument.DocumentStatuses.FirstOrDefault());
            await _dbContext.SaveChangesAsync();

            return new CreateDocumentResponse
            {
                Id = newDocument.Id,
                Status = newDocument.DocumentStatuses.FirstOrDefault().Status.Name

            };
        }

        public override async Task<DeleteDocumentResponse> DeleteDocument(DeleteDocumentRequest request, ServerCallContext context)
        {
            var deletedStatus = from s in _dbContext.Statuses
                                where s.Id == 2
                                select s;
            //var deletedItem = await (from item in _dbContext.Documents
            //                   where item.Id == request.Id
            //                   select item).FirstOrDefaultAsync();

            var deletedItem = await (from item in _dbContext.DocumentStatuses
                                     where item.Id == request.Id
                                     select item).FirstOrDefaultAsync();

            deletedItem.Status = deletedStatus.FirstOrDefault();
            deletedItem.StatusId = deletedStatus.FirstOrDefault().Id;

            //deletedItem.DocumentStatuses.FirstOrDefault().StatusId = deletedStatus.FirstOrDefault().Id;
            //deletedItem.DocumentStatuses.FirstOrDefault().Status = deletedStatus.FirstOrDefault();
            await _dbContext.SaveChangesAsync();

            return new DeleteDocumentResponse
            {
                Status = deletedStatus.FirstOrDefault().Name

            };
        }

        
    }
}
