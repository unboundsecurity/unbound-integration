package main

import (
	"encoding/base64"
	"fmt"

	"github.com/hyperledger/fabric/core/chaincode/shim"
	"github.com/hyperledger/fabric/core/chaincode/shim/ext/cid"
	pb "github.com/hyperledger/fabric/protos/peer"
)

// ExampleChaincode implements shim.Chaincode
type ExampleChaincode struct{}

// Init implements shim.Chaincode.Init
func (chaincode *ExampleChaincode) Init(stub shim.ChaincodeStubInterface) pb.Response {
	return shim.Success(nil)
}

// Invoke implements shim.Chaincode.Invoke
func (chaincode *ExampleChaincode) Invoke(stub shim.ChaincodeStubInterface) pb.Response {
	// Extract user ID
	ci, err := cid.New(stub)
	if err != nil {
		return shim.Error(err.Error())
	}
	encoded, err := ci.GetID()
	if err != nil {
		return shim.Error(err.Error())
	}
	id, err := base64.StdEncoding.DecodeString(encoded)
	if err != nil {
		return shim.Error(err.Error())
	}
	fmt.Printf("User ID: %s\n", id)

	name, args := stub.GetFunctionAndParameters()
	fmt.Printf("Function: %s\nArguments: %v\n", name, args)

	switch name {
	case "put":
		if len(args) != 2 {
			return shim.Error("Incorrect number of arguments for \"put\"")
		}
		key := args[0]
		val := args[1]

		err := stub.PutState(key, []byte(val))

		if err != nil {
			return shim.Error(err.Error())
		}
		fmt.Printf("Result: Success\n\n")
		return shim.Success(nil)

	case "get":
		if len(args) != 1 {
			return shim.Error("Incorrect number of arguments for \"get\"")
		}
		key := args[0]

		val, err := stub.GetState(key)

		if err != nil {
			return shim.Error(err.Error())
		}
		fmt.Printf("Result: Success(%s)\n\n", string(val))
		return shim.Success(val)

	default:
		return shim.Error("Unknown function name: " + name)
	}
}

func main() {
	err := shim.Start(&ExampleChaincode{})
	if err != nil {
		fmt.Printf("Failed to start ExampleChaincode: %s", err)
	}
}
