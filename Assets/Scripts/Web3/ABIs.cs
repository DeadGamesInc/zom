using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ABIs {
    public const string ERC721 =
        @"[{""inputs"":[{""internalType"":""string"",""name"":""_name"",""type"":""string""},{""internalType"":""string"",""name"":""_symbol"",""type"":""string""}],""stateMutability"":""nonpayable"",""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""owner"",""type"":""address""},{""indexed"":true,""internalType"":""address"",""name"":""approved"",""type"":""address""},{""indexed"":true,""internalType"":""uint256"",""name"":""tokenId"",""type"":""uint256""}],""name"":""Approval"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""owner"",""type"":""address""},{""indexed"":true,""internalType"":""address"",""name"":""operator"",""type"":""address""},{""indexed"":false,""internalType"":""bool"",""name"":""approved"",""type"":""bool""}],""name"":""ApprovalForAll"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""to"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""date"",""type"":""uint256""},{""indexed"":false,""internalType"":""address"",""name"":""nft"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""tokenId"",""type"":""uint256""},{""indexed"":false,""internalType"":""string"",""name"":""tokenURI"",""type"":""string""}],""name"":""MintNft"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""previousOwner"",""type"":""address""},{""indexed"":true,""internalType"":""address"",""name"":""newOwner"",""type"":""address""}],""name"":""OwnershipTransferred"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""from"",""type"":""address""},{""indexed"":true,""internalType"":""address"",""name"":""to"",""type"":""address""},{""indexed"":true,""internalType"":""uint256"",""name"":""tokenId"",""type"":""uint256""}],""name"":""Transfer"",""type"":""event""},{""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""adminWhitelist"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""name"":""allTokens"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""name"":""allTokensIndex"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""_to"",""type"":""address""},{""internalType"":""uint256"",""name"":""_tokenId"",""type"":""uint256""}],""name"":""approve"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""_owner"",""type"":""address""}],""name"":""balanceOf"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""balances"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[],""name"":""baseURI"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":""_tokenId"",""type"":""uint256""}],""name"":""getApproved"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""_owner"",""type"":""address""},{""internalType"":""address"",""name"":""_operator"",""type"":""address""}],""name"":""isApprovedForAll"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[],""name"":""maxSupply"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""_to"",""type"":""address""}],""name"":""mint"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[],""name"":""name"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""},{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""operatorApprovals"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""},{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""name"":""ownedTokens"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""name"":""ownedTokensIndex"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[],""name"":""owner"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":""_tokenId"",""type"":""uint256""}],""name"":""ownerOf"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""name"":""owners"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""_to"",""type"":""address""},{""internalType"":""uint256"",""name"":""_quantity"",""type"":""uint256""}],""name"":""preMint"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[],""name"":""renounceOwnership"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""_from"",""type"":""address""},{""internalType"":""address"",""name"":""_to"",""type"":""address""},{""internalType"":""uint256"",""name"":""_tokenId"",""type"":""uint256""}],""name"":""safeTransferFrom"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""_from"",""type"":""address""},{""internalType"":""address"",""name"":""_to"",""type"":""address""},{""internalType"":""uint256"",""name"":""_tokenId"",""type"":""uint256""},{""internalType"":""bytes"",""name"":""_data"",""type"":""bytes""}],""name"":""safeTransferFrom"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""_operator"",""type"":""address""},{""internalType"":""bool"",""name"":""_approved"",""type"":""bool""}],""name"":""setApprovalForAll"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":""_maxSupply"",""type"":""uint256""}],""name"":""setMaxSupply"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""string"",""name"":""_newTokenURI"",""type"":""string""}],""name"":""setTokenURI"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""bytes4"",""name"":""_interfaceId"",""type"":""bytes4""}],""name"":""supportsInterface"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[],""name"":""symbol"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""name"":""tokenApprovals"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":""_index"",""type"":""uint256""}],""name"":""tokenByIndex"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""_owner"",""type"":""address""},{""internalType"":""uint256"",""name"":""_index"",""type"":""uint256""}],""name"":""tokenOfOwnerByIndex"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":""_tokenId"",""type"":""uint256""}],""name"":""tokenURI"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[],""name"":""totalSupply"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""_from"",""type"":""address""},{""internalType"":""address"",""name"":""_to"",""type"":""address""},{""internalType"":""uint256"",""name"":""_tokenId"",""type"":""uint256""}],""name"":""transferFrom"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""newOwner"",""type"":""address""}],""name"":""transferOwnership"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""_admin"",""type"":""address""},{""internalType"":""bool"",""name"":""_isAdmin"",""type"":""bool""}],""name"":""whitelistAdmin"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""}]";
}
