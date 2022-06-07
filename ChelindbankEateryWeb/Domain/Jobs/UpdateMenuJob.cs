using ChelindbankEateryWeb.Domain.Document;
using Quartz;

namespace ChelindbankEateryWeb.Domain.Jobs;

public class UpdateMenuJob: IJob
{
	private DocumentHolder _documentHolder;

	public UpdateMenuJob(DocumentHolder documentHolder)
	{
		_documentHolder = documentHolder;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		await _documentHolder.UpdateDocumentInfo();
	}
}
