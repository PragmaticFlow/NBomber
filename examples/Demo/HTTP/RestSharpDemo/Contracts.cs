namespace Demo.HTTP.RestSharpDemo;

public record User(
    int Id,
    string FirstName,
    string LastName,
    int Age
);

public record UpdateUserReq(
    string FirstName,
    string LastName,
    int Age
);
