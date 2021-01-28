async function fetchWrapper(
    url,
    method,
    refreshUrl,
    body,
    success,
    unreg,
    fail
) {
    const requestOptions = {
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('token') || null}`
        },
        method: method || 'GET',
        body: body || null
    };

    const response = await fetch(url, requestOptions);
    const jsonResponse = response.headers.has('Token-Expired') &&
        response.headers.has('WWW-Authenticate') &&
        'Unhandled exception'/* ||
        await response.json()*/;

    if (response.ok) {
        return await success(jsonResponse);
    }

    if (response.status === 401 && response.headers.has('Token-Expired')) {
        let refreshReqOpt = requestOptions;
        refreshReqOpt = {
            ...requestOptions,
            headers: {
                ...requestOptions.headers,
                'Authorization-Refresh': localStorage.getItem('refreshToken') || null
            },
            method: 'POST'
        };

        const refreshResponse = await fetch(refreshUrl, refreshReqOpt);

        if (!refreshResponse.ok) {
            return await fail('Error while getting refresh token');
        }

        const jsonRefreshResponse = await refreshResponse.json();

        localStorage.setItem('token', jsonRefreshResponse.newJwtToken);
        localStorage.setItem('refreshToken', jsonRefreshResponse.newRefreshToken);

        return await fetchWrapper(
            url,
            method,
            refreshUrl,
            body,
            success,
            unreg,
            fail
        );
    } else if (response.status === 401 && !response.headers.has('Token-Expired')) {
        return await unreg(jsonResponse);
    } else {
        return await fail(jsonResponse);
    }
}