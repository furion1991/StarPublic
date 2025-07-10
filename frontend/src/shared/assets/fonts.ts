import { Exo, Exo_2 } from 'next/font/google'
import localFont from 'next/font/local'

export const fontExo = Exo({
  variable: '--font-exo',
  subsets: ['latin']
})

export const fontExo2 = Exo_2({
  variable: '--font-exo2',
  subsets: ['cyrillic']
})

export const fontRepublicaMinor = localFont({
  src: '../../../public/fonts/RepublicaMinor.ttf',
  variable: '--font-republica-minor'
})
