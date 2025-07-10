import axios from 'axios'
import createAuthRefreshInterceptor from 'axios-auth-refresh'

const API_BASE =
  process.env.NODE_ENV === 'development' ? `/api` : `${process.env.NEXT_PUBLIC_API_BASE}/v1`

export const API = axios.create({
  baseURL: API_BASE,
  withCredentials: true
})

createAuthRefreshInterceptor(API, () => API.post('/auth/refresh'), {
  pauseInstanceWhileRefreshing: true,
  shouldRefresh: (err) => {
    if (err.status !== 401) return false

    if (
      err.response?.data &&
      typeof err.response?.data === 'object' &&
      'error' in err.response?.data
    ) {
      return err.response?.data.error === 'Invalid Password' ? false : true
    }

    return true
  }
})
