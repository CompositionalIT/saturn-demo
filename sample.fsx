#load @".paket\load\net47\main.group.fsx"

open HttpClient
open Thoth.Json.Net

(* 0. Create a couple of helper functions for json serialization *)
let toJson value = Encode.Auto.toString(4, value)
let fromJson<'T> json = Decode.Auto.unsafeFromString<'T> json

(* 1. Basic request / response - HTTP get *)
let responseBody =
    createRequest Get "http://localhost:8085/foo"
    |> getResponse

(* 2. POST with json serialization and deserialization *)

// Our "domain model"
type Department = IT | Finance
type ID = ID of int

type Person =
    { ID : ID
      Name : string
      Age : int
      Department : Department }

// Create a person
let p =
    { ID = ID 123
      Name = sprintf "Joe Bloggs"
      Age = 39
      Department = IT }

// Create another type that looks similar to Person
type Employee = 
    { ID : ID
      Name : int
      Age : int
      Department : Department }

// Convert to json
let personAsRecord = toJson p

// Submit an HTTP post, get the response and then convert the body back to a person
let updatedP =
    createRequest Post "http://localhost:8085/blah"
    |> withHeader (RequestHeader.ContentType "application/json")
    |> withBody personAsRecord
    |> getResponseBody
    |> fromJson<Person>
    // Change from <Person> to <Employee> - see how Thoth gives a nice clear error message

