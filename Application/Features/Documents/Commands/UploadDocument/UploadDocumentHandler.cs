using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.Features.Documents.Commands.UploadDocument
{
    public class UploadDocumentHandler
        : IRequestHandler<UploadDocumentCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public UploadDocumentHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public async Task<Guid> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            var filePath = await _fileStorageService.SaveFileAsync(request.File);

            var document = new Document(
                request.File.FileName,
                filePath
            );

            await _unitOfWork.Document.AddAsync(document);
            await _unitOfWork.CompleteAsync();
            return document.Id;
        }
    }
}
