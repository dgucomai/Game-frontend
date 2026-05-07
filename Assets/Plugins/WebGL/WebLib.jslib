mergeInto(LibraryManager.library, {
  // C#에서 호출할 함수 이름
  GetTableIdFromURL: function () {
    // 1. 브라우저 주소창에서 파라미터 추출
    var urlParams = new URLSearchParams(window.location.search);
    var tableId = urlParams.get('tableId') || "";
    
    // 2. C#은 JS 문자열을 바로 이해 못하므로 메모리 공간을 할당해 넘겨줌
    var bufferSize = lengthBytesUTF8(tableId) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(tableId, buffer, bufferSize);
    
    return buffer;
  },
});