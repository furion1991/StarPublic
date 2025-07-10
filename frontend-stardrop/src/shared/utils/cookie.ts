const ONE_DAY_IN_MILLISECONDS = 24 * 60 * 60 * 1000

export const getCookie = (name: string, source = document) => {
  const v = source?.cookie?.match(`(^|;) ?${name}=([^;]*)(;|$)`)
  return v && v[2] !== 'null' ? v[2] : null
}

export const setCookie = <T>(name: string, value: T, ms?: number) => {
  const expireTime = ms || ONE_DAY_IN_MILLISECONDS

  const d = new Date()
  d.setTime(d.getTime() + expireTime)
  document.cookie = `${name}=${value};path=/;expires=${d.toUTCString()}`
}

export const deleteCookie = (name: string) => {
  setCookie(name, null, -1)
}
