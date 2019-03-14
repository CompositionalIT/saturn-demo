open FSharp.Control.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Saturn
open Thoth.Json.Giraffe

type Department = IT | Finance
type ID = ID of int

type Person =
    { ID : ID
      Name : string
      Age : int
      Department : Department }

let loadPerson pId =
    { ID = ID pId
      Name = sprintf "Person %d" pId
      Age = 39
      Department = IT }

let parsePerson next (ctx:HttpContext) = task {
    let! person = ctx.BindModelAsync<Person>()
    return! json { person with Age = person.Age + 1 } next ctx }

let routes = router {
    get "/foo" (json "HELLO WORLD!")
    getf "/person/%i" (loadPerson >> json)
    post "/blah" parsePerson }

[<EntryPoint>]
let main _ =
    let app = application {
        use_json_serializer (ThothSerializer())
        url "http://0.0.0.0:8085/"
        use_router routes
    }

    run app

    0
