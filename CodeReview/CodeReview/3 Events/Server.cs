/*
    🔻🔻🔻
    Какие плюсы и минусы у представленной реализации событийно-ориентированной архитектуры?
*/

using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeReview._3_Events;

[Table("persons")]
public class PersonData
{
    public long Id { get; set; }

    public string Name { get; set; }
}

public class Person
{
    public long Id { get; set; }

    public string Name { get; set; }
}

public record RenamePersonCommand(long Id, string NewName) : IRequest<bool>;

public class RenamePersonHandler(DbContext dbContext, IMapper mapper)
    : IRequestHandler<RenamePersonCommand, bool>
{
    public async Task<bool> Handle(RenamePersonCommand request, CancellationToken cancellationToken)
    {
        var data = dbContext.Set<PersonData>();

        var person = mapper.Map<Person>(await data.SingleAsync(e => e.Id == request.Id));

        person.Name = request.NewName;

        mapper.Map(person, data);

        return await dbContext.SaveChangesAsync() > 0;
    }
}

public class Controller(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ExecuteAsync(long id, string newName)
    {
        return await mediator.Send(new RenamePersonCommand(id, newName)) ? NoContent() : NotFound();
    }
}