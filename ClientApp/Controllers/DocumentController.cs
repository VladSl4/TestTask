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
        public DocumentController() 
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7173");
            _client = new DocumentGrpc.DocumentGrpcClient(channel);
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
            try
            {
                var response = await _client.GetDocumentByIdAsync(request);
                return Ok(response.Document);
            }
            catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                return NotFound("Document not found or has been deleted");
            }
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
            try
            {
                await _client.DeleteDocumentAsync(request);
                
                return NoContent();
            }
            catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                return NotFound("Document not found");
            }
        }
    }
}

