type SocialProvider = 'tg' | 'vk'

type VkData = {
  state: string
  deviceId: string
  codeVerifier: string
  code: string
}

export type ClaimBonusForSubscriptionProps = {
  provider: SocialProvider
  data?: string
  vkData?: VkData
}
