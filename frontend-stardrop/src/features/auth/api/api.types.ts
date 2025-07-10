export type AuthProps = {
  email: string
  password: string
}

type SocialProvider = 'telegram' | 'vk'

type VkData = {
  state: string
  deviceId: string
  codeVerifier: string
  code: string
}

export type SocialAuthProps = {
  provider: SocialProvider
  data?: string
  vkData?: VkData
}

export type VerifyEmailProps = {
  email: string
  token: string
}
