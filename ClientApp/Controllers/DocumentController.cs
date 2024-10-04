using Microsoft.AspNetCore.Mvc;
using GrpcDocumentService;
using Google.Protobuf.WellKnownTypes;

namespace ClientApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentController: ControllerBase
{
    private readonly DocumentGrpc.DocumentGrpcClient _client;
    public DocumentController(DocumentGrpc.DocumentGrpcClient? client) 
    {
        _client = client;
    }

    [HttpGet]
    public async Task<ICollection<Document>> GetAllDocuments()
    {
        var response = await _client.GetAllDocumentsAsync(new Empty());
        return response.Documents;
    }

    [HttpGet("{id}")]
    public async Task<Document> GetDocumentById(int id)
    {
        var request = new GetDocumentByIdRequest { Id = id };    
        var response = await _client.GetDocumentByIdAsync(request);
        return response.Document;
    }

    [HttpPost]
    public async Task<CreateDocumentResponse> CreateDocument([FromBody] CreateDocumentRequest request)
    {
        var response = await _client.CreateDocumentAsync(request);
        return response;
    }

    [HttpDelete("{id}")]
    public async Task<Empty> DeleteDocument(int id)
    {
        var request = new DeleteDocumentRequest { Id = id };            
        await _client.DeleteDocumentAsync(request);               
        return new Empty();            
    }
}


