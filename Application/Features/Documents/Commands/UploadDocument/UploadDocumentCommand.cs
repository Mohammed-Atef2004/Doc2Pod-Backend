using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Documents.Commands.UploadDocument
{
    public record UploadDocumentCommand(IFormFile File) : IRequest<Guid>;

}
