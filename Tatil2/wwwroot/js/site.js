function submit(event, uri) {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);

    // Fetch kullanarak POST isteği gönderiyoruz.
    fetch(uri, {
        method: 'POST', // HTTP metodunu POST yapıyoruz.
        body: formData, // Form verilerini request body'ye ekliyoruz.
        headers: {
            'Accept': 'application/json', // JSON formatında cevap bekliyoruz.
            // Eğer server tarafında authentication gerekiyorsa token da ekleyebilirsiniz.
            // 'Authorization': 'Bearer ' + token,
        }
    })
        .then(response => response.json()) // Server'dan gelen yanıtı JSON olarak parse et.
        .then(data => {
            // Server'dan gelen yanıtı işleyebiliriz.
            console.log('Success:', data);
        })
        .catch(error => {
            // Hata durumunda yapılacak işlemler.
            console.error('Error:', error);
        });
}