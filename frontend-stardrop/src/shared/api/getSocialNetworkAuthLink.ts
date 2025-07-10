type SocialNetworkName = 'telegram' | 'vk' | 'yandex' | 'steam' | 'google'

const generateRandomString = (length: number) => {
  const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_'
  let result = ''
  for (let i = 0; i < length; i++) {
    result += chars.charAt(Math.floor(Math.random() * chars.length))
  }
  return result
}

const generateCodeChallenge = async (verifier: string) => {
  const encoder = new TextEncoder()
  const data = encoder.encode(verifier)
  const hashBuffer = await crypto.subtle.digest('SHA-256', data)
  const base64 = btoa(String.fromCharCode(...new Uint8Array(hashBuffer)))
    .replace(/\+/g, '-')
    .replace(/\//g, '_')
    .replace(/=+$/, '')
  return base64
}

export const getSocialNetworkAuthLink = (networkName: SocialNetworkName, redirectUri?: string) => {
  const redirectUrl = redirectUri
    ? `https://stardrop.app/${redirectUri}/`
    : 'https://stardrop.app/'

  const getTelegramAuthLink = (redirectUrl: string) => {
    const botId = process.env.NEXT_PUBLIC_TELEGRAM_BOT_ID
    const scope = 'user'
    const nonce = Math.random().toString().substring(7)

    return (
      `https://oauth.telegram.org/auth` +
      `?bot_id=${botId}` +
      `&scope=${scope}` +
      `&nonce=${nonce}` +
      `&origin=${redirectUrl}`
    )
  }

  const getVkAuthLink = async (redirectUrl: string) => {
    const clientId = process.env.NEXT_PUBLIC_VK_PROVIDER_CLIENT_ID
    const state = generateRandomString(32)
    const rawVerifier = generateRandomString(64)
    const codeChallenge = await generateCodeChallenge(rawVerifier)

    localStorage.setItem('state', state)
    localStorage.setItem('codeVerifier', rawVerifier)

    return (
      `https://id.vk.com/authorize` +
      `?response_type=code` +
      `&client_id=${clientId}` +
      `&redirect_uri=${redirectUrl}` +
      `&state=${state}` +
      `&code_challenge=${codeChallenge}` +
      `&code_challenge_method=S256` +
      `&scope=email groups` +
      `&v=5.131`
    )
  }

  switch (networkName) {
    case 'telegram':
      return getTelegramAuthLink(redirectUrl)
    case 'vk':
      return getVkAuthLink(redirectUrl)
    default:
      return null
  }
}
