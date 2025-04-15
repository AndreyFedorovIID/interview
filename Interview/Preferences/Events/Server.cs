/*
    🟢🟢🟢
    Команда начала разрабатывать новый продукт.
    Один из разработчиков предложил следующий шаблон реализации сценариев. 
 
    🔻🔻🔻
    Какие плюсы и минусы у представленной реализации событийно-ориентированной архитектуры?
*/

using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Preferences.Events;

[Table("persons")]
public sealed class PersonData
{
    public long Id { get; set; }

    public string Name { get; set; }
}

public sealed class Person
{
    public long Id { get; set; }

    public string Name { get; set; }
}

public sealed record RenamePersonCommand(long Id, string NewName) : IRequest<bool>;

public sealed class RenamePersonHandler : IRequestHandler<RenamePersonCommand, bool>
{
    public RenamePersonHandler(DbContext dbContext, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(mapper);

        _dbContext = dbContext;
        _mapper = mapper;
    }

    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public async Task<bool> Handle(RenamePersonCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var data = _dbContext.Set<PersonData>();

        var person = _mapper.Map<Person>(
            await data.SingleAsync(e => e.Id == request.Id, cancellationToken));

        person.Name = request.NewName;

        _mapper.Map(person, data);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}

public sealed class Controller : ControllerBase
{
    public Controller(IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);

        _mediator = mediator;
    }

    private readonly IMediator _mediator;

    [HttpPost]
    public async Task<IActionResult> ExecuteAsync(long id, string newName)
        => await _mediator.Send(new RenamePersonCommand(id, newName)) ? NoContent() : NotFound();
}