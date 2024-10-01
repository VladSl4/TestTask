using Microsoft.AspNetCore.Mvc;
using Grpc.Net.Client;
using GrpcDocumentService;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;


namespace ClientApp.Controllers
{
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
        public async Task<ActionResult<List<Document>>> GetAllDocuments()
        {
            var response = await _client.GetAllDocumentsAsync(new Empty());
            return Ok(response.Documents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Document>> GetDocumentById(int id)
        {
            var request = new GetDocumentByIdRequest { Id = id };
            
            var response = await _client.GetDocumentByIdAsync(request);
            return Ok(response.Document);
            
        }

        // Создание нового документа
        [HttpPost]
        public async Task<ActionResult<CreateDocumentResponse>> CreateDocument([FromBody] CreateDocumentRequest request)
        {
            var response = await _client.CreateDocumentAsync(request);
            return CreatedAtAction(nameof(GetDocumentById), new { id = response.Id }, response);
        }

        // Удаление документа
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            var request = new DeleteDocumentRequest { Id = id };
            
             var response = await _client.DeleteDocumentAsync(request);
                
                return Ok(response);
            
        }
    }
}

