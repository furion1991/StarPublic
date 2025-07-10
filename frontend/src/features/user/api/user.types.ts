export type ChangeUserInfoProps = {
  id: string
} & Partial<{
  userName: string
  email: string
  phone: string
}>
